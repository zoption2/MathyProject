using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.UI
{
    public interface ISkillPlanTabView : IView
    {
        public int Grade { get; }
        public ISkillPlanSettingAdapter[] SkillSettings { get; }
    }

    public class SkillPlanTabView : MonoBehaviour, ISkillPlanTabView
    {
        [field: SerializeField] public int Grade { get; private set; }
        [SerializeField] private BaseSkillSettingView[] _skillsSettings;
        [SerializeField] private VerticalLayoutGroup _layoutGroup;

        public ISkillPlanSettingAdapter[] SkillSettings => _skillsSettings;

        public void Show(Action onShow)
        {
            gameObject.SetActive(true);
            onShow?.Invoke();
        }

        public void Hide(Action onHide)
        {
            gameObject.SetActive(false);
            onHide?.Invoke();
        }

        public void Release()
        {
            
        }
    }
}


