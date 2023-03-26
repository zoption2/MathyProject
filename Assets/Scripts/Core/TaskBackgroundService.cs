using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskBackgroundSevice
    {
        UniTask<BackgroundData> GetData<TEnum>(ITaskView view) where TEnum : Enum;
        void Reset();
    }


    public class TaskBackgroundService : ITaskBackgroundSevice 
    {
        private IAddressableRefsHolder refsHolder;
        private System.Random random;
        private Dictionary<Type, BackgroundData> taskBackgrounds;

        public TaskBackgroundService(IAddressableRefsHolder refsHolder)
        {
            this.refsHolder = refsHolder;
            taskBackgrounds = new();
            random = new System.Random();
        }

        public async UniTask<BackgroundData> GetData<TEnum>(ITaskView view) where TEnum : Enum
        {
            var viewType = view.GetType();
            if (!taskBackgrounds.ContainsKey(viewType))
            {
                var values = Enum.GetValues(typeof(TEnum));
                var selected = (TEnum)values.GetValue(random.Next(values.Length));
                var convertedValue = (BackgroundType)Convert.ToInt32(selected);
                var backgroundData = await refsHolder.BackgroundProvider.GetData(convertedValue);
                if (!taskBackgrounds.ContainsKey(viewType))
                {
                    taskBackgrounds.Add(viewType, backgroundData);
                }
            }
            return taskBackgrounds[viewType];
        }

        public void Reset()
        {
            foreach (var data in taskBackgrounds.Values)
            {
                Addressables.Release<Sprite>(data.Sprite);
            }
            taskBackgrounds = new Dictionary<Type, BackgroundData>();
        }
    }
}

