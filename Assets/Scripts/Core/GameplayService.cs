using Cysharp.Threading.Tasks;
using Mathy.Core.Tasks.DailyTasks;
using Mathy.Data;
using Mathy.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    public interface IGameplayService
    {
        public void Prepare(TaskMode mode, List<ScriptableTask> availableTasks);
        public void Start();
    }

    public class GameplayService : IGameplayService
    {
        private const int kSmallModeCount = 10;
        private const int kMediumModeCount = 20;
        private const int kLargeModeCount = 30;
        private const int kMaxTasksLoadedAtOnce = 2;
        private const int kTaskEndDelayMS = 1500;

        private Queue<ITaskController> tasks;
        private ITaskFactory taskFactory;
        private ITaskBackgroundSevice backgroundService;
        private TaskManager taskManager;
        private GameplayScenePointer scenePointer;
        private DataManager dataManager;
        private int remainingTasksCount;
        private int taskIndexer = 0;
        private TaskMode playingMode;
        private List<ScriptableTask> availableTasks;
        public bool IsPractice { get; set; }
        private int TasksInQueue => tasks.Count;

        public GameplayService(ITaskFactory taskFactory, ITaskBackgroundSevice backgroundHandler) 
        {
            this.taskFactory = taskFactory;
            this.backgroundService = backgroundHandler;
        }

        public async void Prepare(TaskMode mode, List<ScriptableTask> availableTasks)
        {
            taskManager = TaskManager.Instance;
            dataManager = DataManager.Instance;
            scenePointer = GameplayScenePointer.Instance;

            playingMode = mode;
            tasks = new(kMaxTasksLoadedAtOnce);
            this.availableTasks = availableTasks;
            remainingTasksCount = GetTasksCountByMode(mode);

            bool isTodayDateExists = await dataManager.IsTodayModeExist(mode);
            if (isTodayDateExists)
            {
                int taskIndex = await dataManager.GetLastTaskIndexOfMode(mode);
                taskIndexer = taskIndex + 1;
                remainingTasksCount -= taskIndexer;
            }

            backgroundService.Reset();
            UpdateCounter();
            UpdateTasksQueue();
        }

        public void Start()
        {
            if (!TryStartTask())
            {
                EndGameplay();
            }
        }

        private bool TryStartTask()
        {
            if (TasksInQueue == 0)
            {
                return false;
            }
            var task = tasks.Dequeue();
            task.StartTask();
            task.ON_COMPLETE += OnTaskComplete;
            task.ON_FORCE_EXIT += ClickOnExitFromGameplay;
            return true;
        }

        private async void OnTaskComplete(ITaskController controller)
        {
            controller.ON_COMPLETE -= OnTaskComplete;
            var result = controller.GetResults();
            result.Mode = playingMode;
            result.TaskModeIndex = taskIndexer;
            taskIndexer++;
            dataManager.SaveTaskData(result);
            UpdateTasksQueue();

            await UniTask.Delay(kTaskEndDelayMS);
            controller.ON_FORCE_EXIT -= ClickOnExitFromGameplay;
            controller.HideAndRelease(()=>
            {
                GameObject.Destroy(controller.ViewParent.gameObject);
            });

            if (!TryStartTask())
            {
                EndGameplay();
            }
        }

        private async void UpdateTasksQueue()
        {
            for (int i = 0; i < kMaxTasksLoadedAtOnce; i++)
            {
                if (remainingTasksCount > 0 && TasksInQueue < 2)
                {
                    var parent = scenePointer.GetNewTaskParent();
                    var task = await taskFactory.CreateTaskFromRange(availableTasks, parent);
                    task.ViewParent = parent;
                    tasks.Enqueue(task);
                    remainingTasksCount--;
                }
            }
        }

        private async void UpdateCounter()
        {
            List<bool> userAnswers = await DataManager.Instance.GetTodayAnswers(playingMode);
            for (int i = 0, j = userAnswers.Count; i < j; i++)
            {
                TaskIndicator indicator = scenePointer.TaskCounterPanel.TaskIndicators[i];
                indicator.Status = userAnswers[i] ? TaskStatus.Right : TaskStatus.Wrong;
            }

            scenePointer.TaskCounterPanel.TaskIndicators[taskIndexer].Status = TaskStatus.InProgress;
        }

        private void SetupPractice()
        {
            scenePointer.TaskCounterPanel.gameObject.SetActive(false);
        }

        private void ClickOnExitFromGameplay()
        {
            EndGameplay();
            TaskManager.Instance.ResetToDefault();
            GameManager.Instance.ChangeState(GameState.MainMenu);
            AdManager.Instance.ShowAdWithProbability(AdManager.Instance.ShowInterstitialAd, 10);
            Debug.Log("Request to TaskService to Exit from gameplay");
        }

        private void EndGameplay()
        {
            backgroundService.Reset();
            foreach (var task in tasks)
            {
                task.ReleaseImmediate();
                GameObject.Destroy(task.ViewParent.gameObject);
            }
            tasks.Clear();
        }

        private int GetTasksCountByMode(TaskMode mode)
        {
            switch (mode)
            {
                case TaskMode.Small: return kSmallModeCount;
                case TaskMode.Medium: return kMediumModeCount;
                case TaskMode.Large: return kLargeModeCount;
                case TaskMode.Challenge:
                case TaskMode.Practic:
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}


