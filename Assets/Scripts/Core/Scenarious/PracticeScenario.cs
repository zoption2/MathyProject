using Cysharp.Threading.Tasks;
using Mathy.Core.Tasks.DailyTasks;
using Mathy.Services;
using UnityEngine;
using Mathy.UI;

namespace Mathy.Core.Tasks
{
    public class PracticeScenario : BaseScenario
    {
        private bool isFailed;
        public override TaskMode TaskMode => TaskMode.Practic;

        protected PracticeScenario(ITaskFactory taskFactory
            , ITaskBackgroundSevice backgroundHandler
            , IAddressableRefsHolder addressableRefs
            , IDataService dataService
            , IPlayerDataService playerDataService
            , IResultScreenMediator resultScreen
            )
            : base(taskFactory, backgroundHandler, addressableRefs, dataService, playerDataService, resultScreen)
        {
        }

        protected override UniTask DoOnStart()
        {
            //isFailed = false;
            //return UniTask.FromResult(isFailed);
            return UniTask.CompletedTask;
        }

        protected override async UniTask UpdateTasksQueue()
        {
            for (int i = 0; i < kMaxTasksLoadedAtOnce; i++)
            {
                //if (isFailed)
                //{
                //    return;
                //}
                if (TasksInQueue < 2)
                {
                    await EnqueueNewTask();
                }
            }
        }

        //protected override void OnTaskComplete(ITaskController controller)
        //{
        //    if (!controller.GetResults().IsAnswerCorrect)
        //    {
        //        isFailed = true;
        //        ClearQueue();
        //    }
        //    base.OnTaskComplete(controller);
        //}

        protected override void ClickOnExitFromGameplay()
        {
            resultScreen.Show(() =>
            {
                ClearTasks();
                GameManager.Instance.ChangeState(GameState.MainMenu);
            });
        }

        protected override void EndGameplay()
        {
            base.EndGameplay();

            resultScreen.CreatePopup(() =>
            {
                GameManager.Instance.ChangeState(GameState.MainMenu);
            });
        }

        private void ClearQueue()
        {
            backgroundService.Reset();
            foreach (var task in tasks)
            {
                task.ReleaseImmediate();
                GameObject.Destroy(task.ViewParent.gameObject);
            }
            tasks.Clear();
        }
    }
}


