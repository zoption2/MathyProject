using Cysharp.Threading.Tasks;
using Mathy.Core.Tasks.DailyTasks;

namespace Mathy.Core.Tasks
{
    public class PracticeScenario : BaseScenario
    {
        public override TaskMode TaskMode => TaskMode.Practic;

        protected PracticeScenario(ITaskFactory taskFactory,
            ITaskBackgroundSevice backgroundHandler,
            IAddressableRefsHolder addressableRefs)
            : base(taskFactory, backgroundHandler, addressableRefs)
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


