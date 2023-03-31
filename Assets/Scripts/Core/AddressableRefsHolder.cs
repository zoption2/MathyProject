﻿using UnityEngine;
using System;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using Mathy.Core.Tasks.DailyTasks;

namespace Mathy
{
    public interface IAddressableRefsHolder
    {
        TaskViewAddressableRef TaskViewProvider { get;}
        UIComponentAddressableRef UIComponentProvider { get; }
        BackgroundAddressableRef BackgroundProvider { get; }
        GameplayScenePopupAddressableRef GameplayScenePopupsProvider { get; }
    }


    [CreateAssetMenu(fileName = "AddressableRefsHolder", menuName = "ScriptableObjects/AddressableRefsHolder")]
    public class AddressableRefsHolder : ScriptableObject, IAddressableRefsHolder
    {
        [field: SerializeField] public TaskViewAddressableRef TaskViewProvider { get; private set; }
        [field: SerializeField] public UIComponentAddressableRef UIComponentProvider { get; private set; }
        [field: SerializeField] public GameplayScenePopupAddressableRef GameplayScenePopupsProvider { get; private set; }
        [field: SerializeField] public BackgroundAddressableRef BackgroundProvider { get; private set; }
    }

    public abstract class AddressableRefsProvider<TType, TRef> where TType : Enum where TRef : AssetReference
    {
        [SerializeField] private RefPair[] references;
        protected DiContainer container;

        public async UniTask<T> InstantiateFromReference<T>(TType type, Transform parent)
        {
            TRef reference = GetReference(type);
            try
            {
                AsyncOperationHandle<GameObject> handler = Addressables.LoadAssetAsync<GameObject>(reference.RuntimeKey);
                await handler;
                GameObject viewPrefab = handler.Result;
                if(container == null)
                {
                    container = ProjectContext.Instance.Container;
                }
                var viewGO = container.InstantiatePrefab(viewPrefab, parent);
                
                //Addressables.ReleaseInstance(handler);
                var view = viewGO.GetComponent<T>();
                return view;
            }
            catch (Exception e)
            {
                throw new ArgumentNullException(
                string.Format("Can't instantiate gameobject by addressable reference for >>{0}<<", type)
                );
            }
        }

        public async UniTask<T> LoadAsync<T>(TType type)
        {
            TRef reference = GetReference(type);
            try
            {
                AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(reference.RuntimeKey);
                await handler;
                return handler.Result;
            }
            catch (Exception)
            {
                throw new ArgumentNullException(
                    string.Format("Can't Load async by addressable reference for >>{0}<<", type)
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
    public class GameplayScenePopupAddressableRef : AddressableRefsProvider<GameplayScenePopup, AssetReferenceGameObject>
    {

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



