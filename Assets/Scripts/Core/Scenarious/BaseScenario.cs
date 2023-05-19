using Cysharp.Threading.Tasks;
using Mathy.Core.Tasks.DailyTasks;
using Mathy.Services;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mathy.UI;

namespace Mathy.Core.Tasks
{
    public interface IScenario
    {
        void StartScenario(List<ScriptableTask> availableTasks);
    }


    public abstract class BaseScenario : IScenario
    {
        protected const int kMaxTasksLoadedAtOnce = 2;
        protected const int kTaskEndDelayMS = 1500;

        protected Queue<ITaskController> tasks;
        protected ITaskController currentTask;
        protected ITaskFactory taskFactory;
        protected ITaskBackgroundSevice backgroundService;
        protected IAddressableRefsHolder addressableRefs;
        protected IDataService dataService;
        protected IAdsService adsService;
        protected IResultScreenMediator resultScreen;
        protected TaskManager taskManager;
        protected GameplayScenePointer scenePointer;
        protected int taskIndexer = 0;
        protected int correctAnswers;
        protected double totalDuration;
        protected List<ScriptableTask> availableTasks;
        protected DailyModeData dailyModeData;

        protected int TasksInQueue => tasks.Count;
        public abstract TaskMode TaskMode { get;}

        public BaseScenario(ITaskFactory taskFactory
            , ITaskBackgroundSevice backgroundHandler
            , IAddressableRefsHolder addressableRefs
            , IDataService dataService
            , IResultScreenMediator resultScreen
            , IAdsService adsService)
        {
            this.taskFactory = taskFactory;
            this.backgroundService = backgroundHandler;
            this.addressableRefs = addressableRefs;
            this.dataService = dataService;
            this.resultScreen = resultScreen;
            this.adsService = adsService;
        }

        protected abstract UniTask DoOnStart();
        protected abstract UniTask UpdateTasksQueue();


        public async void StartScenario(List<ScriptableTask> availableTasks)
        {
            taskManager = TaskManager.Instance;
            scenePointer = GameplayScenePointer.Instance;

            dailyModeData = await dataService.TaskData.GetDailyModeData(DateTime.UtcNow, TaskMode);
            taskIndexer = dailyModeData.PlayedCount;
            correctAnswers = dailyModeData.CorrectAnswers;
            totalDuration = dailyModeData.Duration;
            tasks = new(kMaxTasksLoadedAtOnce);
            this.availableTasks = availableTasks;

            await DoOnStart();

            backgroundService.Reset();
            await UpdateTasksQueue();

            if (tasks.TryPeek(out var task))
            {
                task.Prepare();
            }

            await UniTask.Delay(kTaskEndDelayMS);

            if (!TryStartTask())
            {
                EndGameplay();
            }
        }

        protected virtual async void OnTaskComplete(ITaskController controller)
        {
            controller.ON_COMPLETE -= OnTaskComplete;
            controller.ON_FORCE_EXIT -= ClickOnExitFromGameplay;
            await UpdateResultAndSave(controller);

            await UpdateTasksQueue();

            if(tasks.TryPeek(out var task))
            {
                task.Prepare();
            }

            await UniTask.Delay(kTaskEndDelayMS);

            controller.HideAndRelease(() =>
            {
                currentTask = null;
                if (!TryStartTask())
                {
                    EndGameplay();
                }
                GameObject.Destroy(controller.ViewParent.gameObject);
            });
        }

        protected virtual async UniTask UpdateResultAndSave(ITaskController controller)
        {
            var result = controller.GetResults();
            result.Date = DateTime.Now;
            result.Mode = TaskMode;
            taskIndexer++;
            totalDuration += result.Duration;

            var taskId = await dataService.TaskData.SaveTask(result);

            if (result.IsAnswerCorrect)
            {
                correctAnswers++;
            }

            dailyModeData.IsComplete = false;
            dailyModeData.PlayedCount = taskIndexer;
            dailyModeData.CorrectAnswers = correctAnswers;
            dailyModeData.CorrectRate = (correctAnswers * 100) / taskIndexer;
            dailyModeData.Duration = totalDuration;
            dailyModeData.TasksIds.Add(taskId);

            await dataService.TaskData.UpdateDailyMode(dailyModeData);
        }

        protected virtual bool TryStartTask()
        {
            if (TasksInQueue == 0)
            {
                return false;
            }
            currentTask = tasks.Dequeue();
            currentTask.StartTask();
            currentTask.ON_COMPLETE += OnTaskComplete;
            currentTask.ON_FORCE_EXIT += ClickOnExitFromGameplay;
            return true;
        }

        protected async UniTask EnqueueNewTask()
        {
            var parent = scenePointer.GetNewTaskParent();
            var task = await taskFactory.CreateTaskFromRange(availableTasks, parent);
            task.ViewParent = parent;
            tasks.Enqueue(task);
        }

        protected virtual void ClickOnExitFromGameplay()
        {
            ClearTasks();
            ClearAddressablesCache();
            TaskManager.Instance.ResetToDefault();
            //GameManager.Instance.ChangeState(GameState.MainMenu);
            //AdManager.Instance.ShowAdWithProbability(AdManager.Instance.ShowInterstitialAd, 10);
            Debug.Log("Request to TaskService to Exit from gameplay");
        }

        protected virtual void TryShowInterstitialAds(int probability)
        {
            if (!IAPManager.Instance.IsAdsRemoved())
            {
                adsService.TryShowInterstitialAds(probability);
            }
        }

        protected virtual void EndGameplay()
        {
            ClearAddressablesCache();
        }

        protected virtual void ClearTasks()
        {
            backgroundService.Reset();
            foreach (var task in tasks)
            {
                task.ReleaseImmediate();
                GameObject.Destroy(task.ViewParent.gameObject);
            }
            if (currentTask != null)
            {
                currentTask.ReleaseImmediate();
                GameObject.Destroy(currentTask.ViewParent.gameObject);
            }
            tasks.Clear();
        }

        protected virtual void ClearAddressablesCache()
        {
            addressableRefs.TaskViewProvider.ClearCache();
            addressableRefs.UIComponentProvider.ClearCache();
            addressableRefs.GameplayScenePopupsProvider.ClearCache();
        }
    }
}


