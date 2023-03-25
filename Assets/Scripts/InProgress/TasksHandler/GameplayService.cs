using Cysharp.Threading.Tasks;
using Mathy.Core.Tasks.DailyTasks;
using Mathy.Data;
using System;
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

        private Queue<ITaskController> tasks = new(kMaxTasksLoadedAtOnce);
        private ITaskFactory taskFactory;
        private TaskManager taskManager;
        private DataManager dataManager;
        private int remainingTasksCount;
        private int taskIndexer = 0;
        private TaskMode playingMode;
        private List<ScriptableTask> availableTasks;
        public bool IsPractice { get; set; }
        private int TasksInQueue => tasks.Count;

        public GameplayService(ITaskFactory taskFactory) 
        {
            this.taskFactory = taskFactory;
        }

        public async void Prepare(TaskMode mode, List<ScriptableTask> availableTasks)
        {
            taskManager = TaskManager.Instance;
            dataManager = DataManager.Instance;

            playingMode = mode;
            this.availableTasks = availableTasks;
            remainingTasksCount = GetTasksCountByMode(mode);

            bool isTodayDateExists = await dataManager.IsTodayModeExist(mode);
            if (isTodayDateExists)
            {
                int taskIndex = await dataManager.GetLastTaskIndexOfMode(mode);
                taskIndexer = taskIndex + 1;
                remainingTasksCount -= taskIndexer;
            }

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
                GameObject.Destroy(controller.Parent.gameObject);
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
                    var parent = taskManager.GetNewTaskParent();
                    var task = await taskFactory.CreateTaskFromRange(availableTasks, parent);
                    task.Parent = parent;
                    tasks.Enqueue(task);
                    remainingTasksCount--;
                }
            }
        }

        private void ClickOnExitFromGameplay()
        {
            Debug.Log("Request to TaskService to Exit from gameplay");
        }

        private void EndGameplay()
        {
            foreach (var task in tasks)
            {
                task.ReleaseImmediate();
            }
        }

        //private ScriptableTask GetRandomTaskFromAvailable()
        //{
        //    var random = new System.Random();
        //    var task = availableTasks[random.Next(0, availableTasks.Count)];
        //    return task;
        //}

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


