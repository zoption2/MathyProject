using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskBackgroundSevice
    {
        UniTask<TDecorData> GetData<TEnum, TDecorData>(ITaskView view) where TEnum : Enum where TDecorData : ITaskViewDecorData;
        void Reset();
    }


    public class TaskBackgroundService : ITaskBackgroundSevice 
    {
        private IAddressableRefsHolder refsHolder;
        private System.Random random;
        private Dictionary<Type, ITaskViewDecorData> decors;

        public TaskBackgroundService(IAddressableRefsHolder refsHolder)
        {
            this.refsHolder = refsHolder;
            decors = new();
            random = new System.Random();
        }

        public async UniTask<TDecorData> GetData<TEnum, TDecorData>(ITaskView view)
            where TEnum : Enum
            where TDecorData : ITaskViewDecorData 
        {
            var viewType = view.GetType();

            if (!decors.TryGetValue(viewType, out var decorData))
            {
                var values = Enum.GetValues(typeof(TEnum));
                var selected = (TEnum)values.GetValue(random.Next(values.Length));
                var convertedValue = (BackgroundType)Convert.ToInt32(selected);
                decorData = await refsHolder.BackgroundProvider.GetData<TDecorData>(convertedValue);
                decors[viewType] = decorData;
            }

            return (TDecorData)decors[viewType];
        }

        public void Reset()
        {
            foreach (var data in decors.Values)
            {
                Addressables.Release(data);
            }
            decors = new Dictionary<Type, ITaskViewDecorData>();
        }
    }
}

