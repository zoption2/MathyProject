using UnityEngine;
using Zenject;
using Mathy;
using Mathy.Core.Tasks.DailyTasks;
using Mathy.Core.Tasks;
using Mathy.Data;

public class ProjectContextInstaller : MonoInstaller
{
    [SerializeField] private AddressableRefsHolder refsHolder;

    public override void InstallBindings()
    {
        Container.Bind<IAddressableRefsHolder>().FromInstance(refsHolder).AsSingle();
        Container.Bind<IGameplayService>().To<GameplayService>().AsSingle();
        Container.Bind<ITaskFactory>().To<TaskFactory>().AsSingle();
        Container.Bind<IScenarioFactory>().To<ScenarioFactory>().AsSingle();
        Container.Bind<ITaskViewComponentsProvider>().To<TaskViewComponentsProvider>().AsSingle();
        Container.Bind<ITaskBackgroundSevice>().To<TaskBackgroundService>().AsSingle();

        BindTaskControllers();
        BindScenarious();
    }

    private void BindTaskControllers()
    {
        Container.Bind<DefaultTaskController>().To<DefaultTaskController>().AsTransient();
        Container.Bind<SumOfNumbersTaskController>().To<SumOfNumbersTaskController>().AsTransient();
        Container.Bind<CountToTenImagesTaskController>().To<CountToTenImagesTaskController>().AsTransient();
        Container.Bind<MultipleVariantsTaskController>().To<MultipleVariantsTaskController>().AsTransient();
        Container.Bind<ComparisonBothMissingElementsTaskController>().To<ComparisonBothMissingElementsTaskController>().AsTransient();
        Container.Bind<IsThatTrueTaskController>().To<IsThatTrueTaskController>().AsTransient();
        Container.Bind<WideElementsTaskController>().To<WideElementsTaskController>().AsTransient();
    }

    private void BindScenarious()
    {
        Container.Bind<PracticeScenario>().To<PracticeScenario>().AsSingle();
        Container.Bind<SmallScenario>().To<SmallScenario>().AsSingle();
        Container.Bind<MediumScenario>().To<MediumScenario>().AsSingle();
        Container.Bind<LargeScenario>().To<LargeScenario>().AsSingle();
    }
}




