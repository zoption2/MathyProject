using UnityEngine;
using Zenject;
using Mathy;
using Mathy.Core.Tasks.DailyTasks;
using Mathy.Core.Tasks;
using Mathy.Services;
using Mathy.UI;
using System.Collections.Generic;
using Mathy.Data;
using Mathy.Services.Data;
using Mathy.Services.UI;

public class ProjectContextInstaller : MonoInstaller
{
    [SerializeField] private AddressableRefsHolder refsHolder;
    [SerializeField] private GradeSettingsHolder gradeSettingsHolder;

    public override void InstallBindings()
    {
        Container.Bind<IAddressableRefsHolder>().FromInstance(refsHolder).AsSingle();
        Container.Bind<IGameplayService>().To<GameplayService>().AsSingle();
        Container.Bind<ITaskFactory>().To<TaskFactory>().AsSingle();
        Container.Bind<IScenarioFactory>().To<ScenarioFactory>().AsSingle();
        Container.Bind<ITaskViewComponentsProvider>().To<TaskViewComponentsProvider>().AsSingle();
        Container.Bind<ITaskBackgroundSevice>().To<TaskBackgroundService>().AsSingle();
        Container.Bind<IParentGateService>().To<ParentGateService>().AsSingle();
        Container.Bind<IDataService>().To<DataService>().AsSingle().NonLazy();
        Container.Bind<ISkillPlanService>().To<SkillPlanService>().AsSingle();
        Container.Bind<IUIManager>().To<UIManager>().AsSingle();
        Container.Bind<IAccountService>().To<AccountService>().AsSingle();
        Container.Bind<IAdsService>().To<AdsService>().AsSingle().NonLazy();

        Container.Bind<List<GradeSettings>>().FromInstance(gradeSettingsHolder.GradeSettings).AsSingle();

        BindTaskControllers();
        BindScenarious();
        BindPopupsControllers();
        BindResultScreen();
        BindParentGateScreen();
        BindEnterNamePopup();
    }

    private void BindTaskControllers()
    {
        Container.Bind<DefaultTaskController>().To<DefaultTaskController>().AsTransient();
        Container.Bind<SumOfNumbersTaskController>().To<SumOfNumbersTaskController>().AsTransient();
        Container.Bind<MultipleVariantsTaskController>().To<MultipleVariantsTaskController>().AsTransient();
        Container.Bind<ComparisonBothMissingElementsTaskController>().To<ComparisonBothMissingElementsTaskController>().AsTransient();
        Container.Bind<IsThatTrueTaskController>().To<IsThatTrueTaskController>().AsTransient();
        Container.Bind<WideElementsTaskController>().To<WideElementsTaskController>().AsTransient();

        Container.Bind<CountToTenImagesTaskController>().To<CountToTenImagesTaskController>().AsTransient();
        Container.Bind<SelectFromThreeCountTaskController>().To<SelectFromThreeCountTaskController>().AsTransient();
        Container.Bind<FramesCountToTenTaskController>().To<FramesCountToTenTaskController>().AsTransient();
        Container.Bind<FramesCountToTwentyTaskController>().To<FramesCountToTwentyTaskController>().AsTransient();
        Container.Bind<FramesCountTensAndOnesTaskController>().To<FramesCountTensAndOnesTaskController>().AsTransient();
        Container.Bind<BlocksCountTensAndOnesTaskController>().To<BlocksCountTensAndOnesTaskController>().AsTransient();
    }

    private void BindScenarious()
    {
        Container.Bind<PracticeScenario>().To<PracticeScenario>().AsSingle();
        Container.Bind<SmallScenario>().To<SmallScenario>().AsSingle();
        Container.Bind<MediumScenario>().To<MediumScenario>().AsSingle();
        Container.Bind<LargeScenario>().To<LargeScenario>().AsSingle();
    }

    private void BindPopupsControllers()
    {
        Container.Bind<ParentGatePopupController>().To<ParentGatePopupController>().AsTransient();
    }

    private void BindResultScreen()
    {
        Container.Bind<IResultScreenMediator>().To<ResultScreenMediator>().AsTransient();
        Container.Bind<IResultScreenSkillsController>().To<ResultScreenSkillsController>().AsTransient();
        Container.Bind<IResultScreenAchievementsController>().To<ResultScreenAchievementsController>().AsTransient();
        Container.Bind<IResultScreenRewardController>().To<ResultScreenRewardController>().AsTransient();
        Container.Bind<IResultScreenPlayerInfoController>().To<ResultScreenPlayerInfoController>().AsTransient();
    }

    private void BindParentGateScreen()
    {
        Container.Bind<IParentGatePopupMediator>().To<ParentGatePopupMediator>().AsTransient();
        Container.Bind<IParentGatePopupController>().To<ParentGatePopupController>().AsTransient();
    }

    private void BindEnterNamePopup()
    {
        Container.Bind<IEnterNamePopupMediator>().To<EnterNamePopupMediator>().AsTransient();
        Container.Bind<IEnterNamePopupController>().To<EnterNamePopupController>().AsTransient();
    }
}




