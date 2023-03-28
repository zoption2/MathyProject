using Cysharp.Threading.Tasks;
using Mathy.Core.Tasks.DailyTasks;

namespace Mathy.Core.Tasks
{
    public class PracticeScenario : BaseScenario
    {
        public override TaskMode TaskMode => TaskMode.Practic;

        public PracticeScenario(ITaskFactory taskFactory
            , ITaskBackgroundSevice backgroundHandler
            , ITaskViewComponentsProvider componentsProvider)
            : base(taskFactory, backgroundHandler, componentsProvider)
        {
        }

        protected override async UniTask DoOnStart()
        {
            
        }

        protected override async UniTask UpdateTasksQueue()
        {
            for (int i = 0; i < kMaxTasksLoadedAtOnce; i++)
            {
                if (TasksInQueue < 2)
                {
                    await EnqueueNewTask();
                }
            }
        }

        protected override void EndGameplay()
        {
            throw new System.NotImplementedException();
        }
    }
}


