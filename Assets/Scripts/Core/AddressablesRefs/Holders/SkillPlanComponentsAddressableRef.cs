using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Mathy
{
    public interface ISkillPlanComponentsAddressableRef : IAddressableRefsProvider
    {
        UniTask<T> InstantiateFromReference<T>(SkillPlanPopupComponents type, Transform parent);
    }

    [Serializable]
    public class SkillPlanComponentsAddressableRef : AddressableRefsProvider<SkillPlanPopupComponents, AssetReferenceGameObject>
    , ISkillPlanComponentsAddressableRef
    {

    }
}



