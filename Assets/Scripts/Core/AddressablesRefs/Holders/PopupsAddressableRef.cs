using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Mathy
{
    public interface IPopupsAddressableRefProvider : IAddressableRefsProvider
    {
        UniTask<T> InstantiateFromReference<T>(Popups type, Transform parent);
    }

    [Serializable]
    public class PopupsAddressableRef : AddressableRefsProvider<Popups, AssetReferenceGameObject>
        , IPopupsAddressableRefProvider
    { 

    }
}



