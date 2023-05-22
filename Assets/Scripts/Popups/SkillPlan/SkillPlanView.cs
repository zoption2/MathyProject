using Mathy.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.UI
{
    public interface ISkillPlanView : IView
    {
        public event Action ON_CLOSE_CLICK;
        public event Action<int> ON_GRADE_TAB_SWITCHED;
        public event Action<int> ON_GRADE_CHANGED;
    }

    public class SkillPlanView : MonoBehaviour
    {
        public event Action ON_CLOSE_CLICK;
        public event Action<int> ON_SWITCH_TAB_CLICK;

        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _selectAllText;
        [SerializeField] private SkillPlanTabView[] _tabs;
        [SerializeField] private Toggle[] _toggles;
        [SerializeField] private Button[] _gradeTabButtons;
        [SerializeField] private Button _selectAllButton;
        [SerializeField] private Button _infoButton;


    }


    public interface ISkillPlanTabView : IView
    {
        public int Grade { get; }
        public ISkillPlanSettingAdapter[] SkillSettings { get; }
    }

    public class SkillPlanTabView : MonoBehaviour, ISkillPlanTabView
    {
        [field: SerializeField] public int Grade { get; private set; }
        [SerializeField] private Transform _skillsHolder;
        [SerializeField] private VerticalLayoutGroup _layoutGroup;

        public ISkillPlanSettingAdapter[] SkillSettings => throw new NotImplementedException();

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

    public interface ISkillPlanSettingAdapter
    {
        event Action<bool, int> ON_ACTIVATE;
        void Init(bool isActive, int value);
        int Value { get; }
        bool IsEnable { get; }
        SkillType SkillType { get; }
    }
}


