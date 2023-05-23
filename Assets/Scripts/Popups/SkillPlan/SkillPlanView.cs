using Mathy.Data;
using System;
using System.Collections.Generic;
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
        public ISkillPlanTabView[] GradeTabs { get; }
    }

    public class SkillPlanView : MonoBehaviour, ISkillPlanView
    {
        public event Action ON_CLOSE_CLICK;
        public event Action<int> ON_SWITCH_TAB_CLICK;
        public event Action<int> ON_GRADE_TAB_SWITCHED;
        public event Action<int> ON_GRADE_CHANGED;

        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _selectAllText;
        [SerializeField] private SkillPlanTabView[] _tabs;
        [SerializeField] private SwitchGradeGroup[] _gradeSwitchers;
        [SerializeField] private Button _selectAllButton;
        [SerializeField] private Button _infoButton;

        [SerializeField] private VerticalLayoutGroup _gradeButtonsLayout;
        [SerializeField] private BaseViewAnimator _animator;

        public ISkillPlanTabView[] GradeTabs => _tabs;

        public void Show(Action onShow)
        {
            _animator.AnimateShowing(() =>
            {
                SubscribeTabsButtons();
                onShow?.Invoke();
            });
        }

        public void Hide(Action onHide)
        {
            _animator.AnimateHiding(() =>
            {
                onHide?.Invoke();
            });
        }

        public void Release()
        {
            Destroy(gameObject);
        }

        private void SubscribeTabsButtons()
        {
            for (int i = 0, j = _gradeSwitchers.Length; i < j; i++)
            {
                _gradeSwitchers[i].ON_GRADE_TAB_SWITCH += DoOnSwitchTabClick;
                _gradeSwitchers[i].ON_GRADE_TOGGLE_SWITCH += DoOnToggleSwitch;
            }
        }

        private void UnsubscribeTabsButtons()
        {
            for (int i = 0, j = _gradeSwitchers.Length; i < j; i++)
            {
                _gradeSwitchers[i].ON_GRADE_TAB_SWITCH -= DoOnSwitchTabClick;
                _gradeSwitchers[i].ON_GRADE_TOGGLE_SWITCH -= DoOnToggleSwitch;
            }
        }

        private void DoOnSwitchTabClick(int index)
        {

        }

        private void DoOnToggleSwitch(int index, bool isActive)
        {

        }
    }

}
