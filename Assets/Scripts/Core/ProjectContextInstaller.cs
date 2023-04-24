using UnityEngine;
using Zenject;
using Mathy;
using Mathy.Core.Tasks.DailyTasks;
using Mathy.Core.Tasks;
using Mathy.Services;
using Mathy.UI;
using System.Collections.Generic;
using Mathy.Data;

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

        Container.Bind<List<GradeSettings>>().FromInstance(gradeSettingsHolder.GradeSettings).AsSingle();

        BindTaskControllers();
        BindScenarious();
        BindPopupsControllers();
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
}




