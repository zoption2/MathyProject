using Cysharp.Threading.Tasks;
using Mathy.Core.Tasks.DailyTasks;
using Mathy.Data;
using Mathy.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    public abstract class DailyTaskScenario : BaseScenario
    {
        private int remainingTasksCount;
        private ITaskCounter counterView;

        protected abstract int TotalTasks { get; }

        protected DailyTaskScenario(ITaskFactory taskFactory,
            ITaskBackgroundSevice backgroundHandler,
            ITaskViewComponentsProvider componentsProvider) 
            : base(taskFactory, backgroundHandler, componentsProvider)
        {
        }


        protected async override UniTask DoOnStart()
        {
            remainingTasksCount = TotalTasks;
            var counterParent = scenePointer.CounterParent;
            counterView = await componentsProvider.GetUIComponentAsync<ITaskCounter>(UIComponentType.DefaultCounter, counterParent);
            bool isTodayDateExists = await dataManager.IsTodayModeExist(TaskMode);
            if (isTodayDateExists)
            {
                int taskIndex = await dataManager.GetLastTaskIndexOfMode(TaskMode);
                taskIndexer = taskIndex + 1;
                remainingTasksCount -= taskIndexer;
            }

            InitCounter();
        }

        protected override void OnTaskComplete(ITaskController controller)
        {
            var isAnswerCorrect = controller.GetResults().IsAnswerCorrect;
            TaskStatus status = isAnswerCorrect ? TaskStatus.Right : TaskStatus.Wrong;
            counterView.ChangeStatusByIndex(taskIndexer, status);
            base.OnTaskComplete(controller);
        }

        protected override bool TryStartTask()
        {
            var temp = base.TryStartTask();
            counterView.SetCurrentCount(taskIndexer);
            counterView.ChangeStatusByIndex(taskIndexer, TaskStatus.InProgress);
            return temp;
        }

        protected async override UniTask UpdateTasksQueue()
        {
            for (int i = 0; i < kMaxTasksLoadedAtOnce; i++)
            {
                if (remainingTasksCount > 0 && TasksInQueue < 2)
                {
                    await EnqueueNewTask();
                    remainingTasksCount--;
                }
            }
        }

        protected override void EndGameplay()
        {
            GameObject.Destroy(counterView.gameObject);
            var resultsView = scenePointer.ResultsWindow;
            resultsView.gameObject.SetActive(true);
            float correctRate = correctAnswers / (float)TotalTasks * 100f;
            resultsView.DisplayResult(correctAnswers, TotalTasks, correctRate, false);
        }

        protected async void InitCounter()
        {
            List<bool> userAnswers = await DataManager.Instance.GetTodayAnswers(TaskMode);
            counterView.Init(TotalTasks, userAnswers);

            for (int i = 0, j = userAnswers.Count; i < j; i++)
            {
                var isCorrect = userAnswers[i];
                if (isCorrect)
                {
                    correctAnswers++;
                }
            }
        }
    }


    public class SmallScenario : DailyTaskScenario
    {
        public override TaskMode TaskMode => TaskMode.Small;
        protected override int TotalTasks => 10;


        public SmallScenario(ITaskFactory taskFactory
            , ITaskBackgroundSevice backgroundHandler
            , ITaskViewComponentsProvider componentsProvider) 
            : base(taskFactory, backgroundHandler, componentsProvider)
        {
        }
    }


    public class MediumScenario : DailyTaskScenario
    {
        public override TaskMode TaskMode => TaskMode.Medium;
        protected override int TotalTasks => 20;


        public MediumScenario(ITaskFactory taskFactory
            , ITaskBackgroundSevice backgroundHandler
            , ITaskViewComponentsProvider componentsProvider)
            : base(taskFactory, backgroundHandler, componentsProvider)
        {
        }
    }


    public class LargeScenario : DailyTaskScenario
    {
        public override TaskMode TaskMode => TaskMode.Large;
        protected override int TotalTasks => 30;


        public LargeScenario(ITaskFactory taskFactory
            , ITaskBackgroundSevice backgroundHandler
            , ITaskViewComponentsProvider componentsProvider)
            : base(taskFactory, backgroundHandler, componentsProvider)
        {
        }
    }
}


