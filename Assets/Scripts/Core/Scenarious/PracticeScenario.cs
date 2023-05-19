using Cysharp.Threading.Tasks;
using Mathy.Core.Tasks.DailyTasks;
using Mathy.Services;
using UnityEngine;
using Mathy.UI;

namespace Mathy.Core.Tasks
{
    public class PracticeScenario : BaseScenario
    {
        public override TaskMode TaskMode => TaskMode.Practic;

        protected PracticeScenario(ITaskFactory taskFactory
            , ITaskBackgroundSevice backgroundHandler
            , IAddressableRefsHolder addressableRefs
            , IDataService dataService
            , IResultScreenMediator resultScreen
            , IAdsService adsService
            )
            : base(taskFactory, backgroundHandler, addressableRefs, dataService, resultScreen, adsService)
        {
        }

        protected override UniTask DoOnStart()
        {
            return UniTask.CompletedTask;
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

        protected override void ClickOnExitFromGameplay()
        {
            base.ClickOnExitFromGameplay();
            EndGameplay();
        }

        protected override void EndGameplay()
        {
            base.EndGameplay();
            TryShowInterstitialAds(35);
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


