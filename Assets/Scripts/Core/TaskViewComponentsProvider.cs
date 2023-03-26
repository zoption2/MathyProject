using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskViewComponentsProvider
    {
        UniTask<TComponent> GetUIComponentAsync<TComponent>(UIComponentType type, Transform parent);
        UniTask<ITaskViewComponent> GetUIComponentAsync(UIComponentType type, Transform parent);
    }

    public class TaskViewComponentsProvider : ITaskViewComponentsProvider
    {
        private IAddressableRefsHolder refsHolder;

        public TaskViewComponentsProvider(IAddressableRefsHolder holder)
        {
            refsHolder = holder;
        }

        public async UniTask<TComponent> GetUIComponentAsync<TComponent>(UIComponentType type, Transform parent)
        {
            return await refsHolder.UIComponentProvider.InstantiateFromReference<TComponent>(type, parent);
        }

        public async UniTask<ITaskViewComponent> GetUIComponentAsync(UIComponentType type, Transform parent)
        {
            return await refsHolder.UIComponentProvider.InstantiateFromReference<ITaskViewComponent>(type, parent);
        }
    }
}

