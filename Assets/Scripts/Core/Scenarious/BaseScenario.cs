using Cysharp.Threading.Tasks;
using Mathy.Core.Tasks.DailyTasks;
using Mathy.Data;
using System.Collections.Generic;
using UnityEngine;

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
        protected TaskManager taskManager;
        protected GameplayScenePointer scenePointer;
        protected DataManager dataManager;
        protected int taskIndexer = 0;
        protected int correctAnswers;
        protected List<ScriptableTask> availableTasks;

        protected int TasksInQueue => tasks.Count;
        public abstract TaskMode TaskMode { get;}

        public BaseScenario(ITaskFactory taskFactory
            , ITaskBackgroundSevice backgroundHandler
            , IAddressableRefsHolder addressableRefs)
        {
            this.taskFactory = taskFactory;
            this.backgroundService = backgroundHandler;
            this.addressableRefs = addressableRefs;
        }

        protected abstract UniTask DoOnStart();
        protected abstract UniTask UpdateTasksQueue();
        protected abstract void EndGameplay();

        public async virtual void StartScenario(List<ScriptableTask> availableTasks)
        {
            taskManager = TaskManager.Instance;
            dataManager = DataManager.Instance;
            scenePointer = GameplayScenePointer.Instance;

            correctAnswers = 0;
            taskIndexer = 0;
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
            var result = controller.GetResults();
            result.Mode = TaskMode;
            taskIndexer++;
            result.TaskModeIndex = taskIndexer;
            dataManager.SaveTaskData(result);

            if (result.IsAnswerCorrect)
            {
                correctAnswers++;
            }

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
            TaskManager.Instance.ResetToDefault();
            GameManager.Instance.ChangeState(GameState.MainMenu);
            AdManager.Instance.ShowAdWithProbability(AdManager.Instance.ShowInterstitialAd, 10);
            Debug.Log("Request to TaskService to Exit from gameplay");
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
    }
}


