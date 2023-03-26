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
        Container.Bind<ITaskViewComponentsProvider>().To<TaskViewComponentsProvider>().AsSingle();
        Container.Bind<ITaskBackgroundSevice>().To<TaskBackgroundService>().AsSingle();

        BindTaskControllers();
    }

    private void BindTaskControllers()
    {
        Container.Bind<AdditionTaskController>().To<AdditionTaskController>().AsTransient();
    }
}




