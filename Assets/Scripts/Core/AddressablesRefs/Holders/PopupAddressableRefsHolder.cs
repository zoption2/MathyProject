using UnityEngine;


namespace Mathy.Data.Addressables
{
    public interface IPopupAddressableRefsHolder
    {
        IPopupsAddressableRefProvider Main { get; }
        ISkillPlanComponentsAddressableRef SkillPlanComponents { get; }
    }

    [CreateAssetMenu(fileName = "PopupRefsHolder", menuName = "ScriptableObjects/AddressableRefsHolders/Popups")]
    public class PopupAddressableRefsHolder : ScriptableObject, IPopupAddressableRefsHolder
    {
        [SerializeField] private PopupsAddressableRef _popupsProvider;
        [SerializeField] private SkillPlanComponentsAddressableRef _skillPlanComponentsProvider;


        public IPopupsAddressableRefProvider Main => _popupsProvider;
        public ISkillPlanComponentsAddressableRef SkillPlanComponents => _skillPlanComponentsProvider;
    }
}



