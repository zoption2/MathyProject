using Cysharp.Threading.Tasks;
using Mathy.Core.Tasks.DailyTasks;
using Mathy.Services;
using Mathy.UI;
using System;
using System.Linq;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    public abstract class DailyTaskScenario : BaseScenario
    {
        private int remainingTasksCount;
        private ITaskCounter counterView;

        protected abstract int TotalTasks { get; }

        protected DailyTaskScenario(ITaskFactory taskFactory
            , ITaskBackgroundSevice backgroundHandler
            , IAddressableRefsHolder addressableRefs
            , IDataService dataService
            , IResultScreenMediator resultScreen
            ) 
            : base(taskFactory, backgroundHandler, addressableRefs, dataService, resultScreen)
        {
        }


        protected async override UniTask DoOnStart()
        {
            dailyModeData.TotalTasks = TotalTasks;
            remainingTasksCount = TotalTasks;
            var counterParent = scenePointer.CounterParent;
            counterView = await addressableRefs.GameplayScenePopupsProvider.InstantiateFromReference<ITaskCounter>(TaskFeatures.CounterVariantOne, counterParent);
            remainingTasksCount -= taskIndexer;

            InitCounter();
        }

        protected override async UniTask UpdateResultAndSave(ITaskController controller)
        {
            var results = controller.GetResults();
            results.Date = DateTime.UtcNow;
            var isModeDone = (remainingTasksCount == 0 && TasksInQueue == 0);
            TaskStatus status = results.IsAnswerCorrect ? TaskStatus.Right : TaskStatus.Wrong;
            counterView.ChangeStatusByIndex(taskIndexer, status);

            results.Mode = TaskMode;
            taskIndexer++;
            totalDuration += results.Duration;

            var taskId = await dataService.TaskData.SaveTask(results);

            if (results.IsAnswerCorrect)
            {
                correctAnswers++;
            }

            dailyModeData.IsComplete = isModeDone;
            dailyModeData.PlayedCount = taskIndexer;
            dailyModeData.CorrectAnswers = correctAnswers;
            dailyModeData.CorrectRate = (correctAnswers * 100) / taskIndexer;
            dailyModeData.Duration = totalDuration;
            dailyModeData.TasksIds.Add(taskId);

            await dataService.TaskData.UpdateDailyMode(dailyModeData);
        }

        protected override bool TryStartTask()
        {
            var temp = base.TryStartTask();
            counterView.SetCurrentCount(taskIndexer + 1);
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

        protected async override void EndGameplay()
        {
            base.EndGameplay();
            TryShowASD();
            var gainedExperience = PointsHelper.GetExperiencePointsByRate(dailyModeData.CorrectRate);
            await dataService.PlayerData.Progress.AddExperienceAsync(gainedExperience);

            var totalExp = await dataService.PlayerData.Progress.GetPlayerExperienceAsync();
            var rank = PointsHelper.GetRankByExperience(totalExp);
            await dataService.PlayerData.Progress.SaveRankAsynk(rank);

            resultScreen.CreatePopup(()=>
            {
                GameObject.Destroy(counterView.gameObject);
                ScenesManager.Instance.DisableTaskScene();
            });
            resultScreen.ON_CLOSE_CLICK += ChangeScene;
           // resultScreen.ON_CLOSE_CLICK += TryShowASD;
        }

        private void ChangeScene()
        {
            resultScreen.ON_CLOSE_CLICK -= ChangeScene;
            GameManager.Instance.ChangeState(GameState.MainMenu);
        }

        private void TryShowASD()
        {
            resultScreen.ON_CLOSE_CLICK -= TryShowASD;
            AdManager.Instance.ShowAdWithProbability(AdManager.Instance.ShowInterstitialAd, 30);
        }

        protected override void ClickOnExitFromGameplay()
        {
            base.ClickOnExitFromGameplay();
            TryShowASD();
            resultScreen.CreatePopup(() =>
            {
                GameManager.Instance.ChangeState(GameState.MainMenu);
                GameObject.Destroy(counterView.gameObject);
            });
        }

        protected async void InitCounter()
        {
            var tasks = await dataService.TaskData.GetResultsByModeAndDate(TaskMode, DateTime.UtcNow);
            var userAnswers = tasks.Select(x => x.IsAnswerCorrect).ToList();
            counterView.Init(TotalTasks, userAnswers);

            //for (int i = 0, j = userAnswers.Count; i < j; i++)
            //{
            //    var isCorrect = userAnswers[i];
            //    if (isCorrect)
            //    {
            //        correctAnswers++;
            //    }
            //}
        }
    }


    public class SmallScenario : DailyTaskScenario
    {
        public override TaskMode TaskMode => TaskMode.Small;
        protected override int TotalTasks => 10;


        protected SmallScenario(ITaskFactory taskFactory
            , ITaskBackgroundSevice backgroundHandler
            , IAddressableRefsHolder addressableRefs
            , IDataService dataService
            , IResultScreenMediator resultScreen
            )
            : base(taskFactory, backgroundHandler, addressableRefs, dataService, resultScreen)
        {
        }
    }


    public class MediumScenario : DailyTaskScenario
    {
        public override TaskMode TaskMode => TaskMode.Medium;
        protected override int TotalTasks => 20;


        protected MediumScenario(ITaskFactory taskFactory
            , ITaskBackgroundSevice backgroundHandler
            , IAddressableRefsHolder addressableRefs
            , IDataService dataService
            , IResultScreenMediator resultScreen
            )
            : base(taskFactory, backgroundHandler, addressableRefs, dataService, resultScreen)
        {
        }
    }


    public class LargeScenario : DailyTaskScenario
    {
        public override TaskMode TaskMode => TaskMode.Large;
        protected override int TotalTasks => 30;


        protected LargeScenario(ITaskFactory taskFactory
            , ITaskBackgroundSevice backgroundHandler
            , IAddressableRefsHolder addressableRefs
            , IDataService dataService
            , IResultScreenMediator resultScreen
            )
            : base(taskFactory, backgroundHandler, addressableRefs, dataService, resultScreen)
        {
        }
    }
}


