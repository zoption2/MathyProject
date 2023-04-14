using UnityEngine;
using System;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using Mathy.Core.Tasks.DailyTasks;
using System.Linq;
using System.Collections.Concurrent;

namespace Mathy
{
    public interface IAddressableRefsHolder
    {
        TaskViewAddressableRef TaskViewProvider { get;}
        UIComponentAddressableRef UIComponentProvider { get; }
        BackgroundAddressableRef BackgroundProvider { get; }
        TaskFeaturesAddressableRef GameplayScenePopupsProvider { get; }
        PopupsAddressableRef PopupsProvider { get; }
        TaskCountedImageAdressableRef TaskCountedImageProvider { get; }
    }


    [CreateAssetMenu(fileName = "AddressableRefsHolder", menuName = "ScriptableObjects/AddressableRefsHolder")]
    public class AddressableRefsHolder : ScriptableObject, IAddressableRefsHolder
    {
        [field: SerializeField] public TaskViewAddressableRef TaskViewProvider { get; private set; }
        [field: SerializeField] public UIComponentAddressableRef UIComponentProvider { get; private set; }
        [field: SerializeField] public TaskFeaturesAddressableRef GameplayScenePopupsProvider { get; private set; }
        [field: SerializeField] public BackgroundAddressableRef BackgroundProvider { get; private set; }
        [field: SerializeField] public TaskCountedImageAdressableRef TaskCountedImageProvider { get; private set; }
        [field: SerializeField] public PopupsAddressableRef PopupsProvider { get; private set; }
    }

    public abstract class AddressableRefsProvider<TType, TRef> where TType : Enum where TRef : AssetReference
    {
        [SerializeField] protected RefPair[] references;
        protected DiContainer container;
        private ConcurrentDictionary<string, AsyncOperationHandle<GameObject>> cache = new();

        public async UniTask<T> InstantiateFromReference<T>(TType type, Transform parent)
        {
            TRef reference = GetReference(type);
            string key = reference.RuntimeKey.ToString();
            GameObject viewPrefab = null;
            if (cache.TryGetValue(key, out AsyncOperationHandle<GameObject> handle))
            {
                viewPrefab = handle.Result;
                cache.TryUpdate(key, handle, handle);
            }
            else
            {
                try
                {
                    AsyncOperationHandle<GameObject> handler = Addressables.LoadAssetAsync<GameObject>(reference.RuntimeKey);
                    await handler;
                    int attempts = 0;
                    while (handler.Status != AsyncOperationStatus.Succeeded && attempts < 5)
                    {
                        handler = Addressables.LoadAssetAsync<GameObject>(reference.RuntimeKey);
                        await handler;
                        attempts++;
                        Debug.LogFormat("Invoked attempt № {0} for {1}", attempts, type);
                    }
                    cache.TryAdd(key, handler);
                    viewPrefab = handler.Result;
                    UnityEngine.Debug.LogFormat("Cache contains {0} references", cache.Count);
                }
                catch (Exception e)
                {
                    throw new ArgumentNullException(
                    string.Format("Can't instantiate gameobject by addressable reference for >>{0}<<", type)
                    );
                }
            }

            if (container == null)
            {
                container = ProjectContext.Instance.Container;
            }
            var viewGO = container.InstantiatePrefab(viewPrefab, parent);
            var view = viewGO.GetComponent<T>();
            return view;
        }

        public void ClearCache()
        {
            foreach (var reference in cache.Values)
            {
                Addressables.Release(reference);
            }
            cache.Clear();
        }

        public async UniTask<T> LoadAsync<T>(TType type)
        {
            TRef reference = GetReference(type);
            try
            {
                AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(reference.RuntimeKey);
                await handler;
                int attempts = 0;
                while (handler.Status != AsyncOperationStatus.Succeeded && attempts < 5)
                {
                    handler = Addressables.LoadAssetAsync<T>(reference.RuntimeKey);
                    await handler;
                    attempts++;
                    Debug.LogFormat("Invoked attempt № {0} for {1}", attempts, type);
                }
                return handler.Result;
            }
            catch (Exception)
            {
                throw new ArgumentNullException(
                    string.Format("Can't Load async by addressable reference for >>{0}<<", type)
                    );
            }
        }

        protected async UniTask<T> LoadByRefAsync<T>(TRef reference)
        {
            try
            {
                AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(reference.RuntimeKey);
                await handler;
                int attempts = 0;
                while (handler.Status != AsyncOperationStatus.Succeeded && attempts < 5)
                {
                    handler = Addressables.LoadAssetAsync<T>(reference.RuntimeKey);
                    await handler;
                    attempts++;
                    Debug.LogFormat("Invoked attempt № {0} for {1}", attempts, typeof(T));
                }
                return handler.Result;
            }
            catch (Exception)
            {
                throw new ArgumentNullException(
                    string.Format("Can't LoadByRefAsync")
                    );
            }
        }

        private TRef GetReference(TType type)
        {
            for (int i = 0, j = references.Length; i < j; i++)
            {
                if (references[i].Type.Equals(type))
                {
                    return references[i].Reference;
                }
            }
            throw new ArgumentException(
                string.Format("There is no addressable reference finded for task >>{0}<<", type)
                );
        }

        [Serializable]
        public class RefPair
        {
            [field: SerializeField] public TType Type { get; private set; }
            [field: SerializeField] public TRef Reference { get; private set; }
        }
    }

    [Serializable]
    public class TaskViewAddressableRef : AddressableRefsProvider<TaskType, AssetReferenceGameObject>
    {

    }

    [Serializable]
    public class UIComponentAddressableRef : AddressableRefsProvider<UIComponentType, AssetReferenceGameObject>
    {

    }

    [Serializable]
    public class TaskFeaturesAddressableRef : AddressableRefsProvider<TaskFeatures, AssetReferenceGameObject>
    {

    }

    [Serializable]
    public class PopupsAddressableRef : AddressableRefsProvider<Popups, AssetReferenceGameObject>
    { 

    }


    [Serializable]
    public class TaskCountedImageAdressableRef : AddressableRefsProvider<CountedImageType, AssetReferenceSprite>
    {
        public async UniTask<Sprite> GetRandomSprite()
        {
            var random = new System.Random();
            var values = Enum.GetValues(typeof(CountedImageType));
            var type = (CountedImageType)values.GetValue(random.Next(values.Length));
            return await LoadAsync<Sprite>(type);
        }

        public async UniTask<Sprite> GetSpriteByType(CountedImageType type)
        {
            var random = new System.Random();
            var availableRefs = references.Where(x => x.Type == type).Select(x => x.Reference).ToArray();
            var randomIndex = random.Next(0, availableRefs.Length);
            var persistRef = availableRefs[randomIndex];
            return await LoadByRefAsync<Sprite>(persistRef);
        }
    }



    [Serializable]
    public class BackgroundAddressableRef
    {
        [SerializeField] private ReferenceData[] references;

        public async UniTask<TDecor> GetRandomData<TDecor>() where TDecor : ITaskViewDecorData
        {
            var random = new System.Random();
            var values = Enum.GetValues(typeof(BackgroundType));
            var back = (BackgroundType)values.GetValue(random.Next(values.Length));
            return await GetData<TDecor>(back);
        }

        public async UniTask<TDecor> GetData<TDecor>(BackgroundType type) where TDecor : ITaskViewDecorData
        {
            var referenceData = GetReference(type);

            try
            {
                AsyncOperationHandle<TDecor> handler = Addressables.LoadAssetAsync<TDecor>(referenceData.Reference.RuntimeKey);
                await handler;
                return handler.Result;
            }
            catch (Exception)
            {
                throw new ArgumentNullException(
                    string.Format("Can't load ScriptableObject from addressable reference for >>{0}<<", type)
                    );
            }
        }

        private ReferenceData GetReference(BackgroundType type)
        {
            for (int i = 0, j = references.Length; i < j; i++)
            {
                if (references[i].Type.Equals(type))
                {
                    return references[i];
                }
            }
            throw new ArgumentException(
                string.Format("There is no addressable reference finded for task >>{0}<<", type)
                );
        }

        [Serializable]
        private class ReferenceData
        {
            [field: SerializeField] public BackgroundType Type { get; private set; }
            [field: SerializeField] public AssetReference Reference { get; private set; }
        }
    }
}



