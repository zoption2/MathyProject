using Mathy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.UI
{
    public interface ISkillPlanPopupView : IView
    {
        public Transform SettingsHolder { get; }
    }

    public class SkillPlanPopupView : MonoBehaviour, ISkillPlanPopupView
    {
        public event Action ON_CLOSE_CLICK;
        public event Action<int> ON_GRADE_TAB_SWITCHED;
        public event Action<int, bool> ON_GRADE_TOGGLE_CHANGED;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _selectAllText;
        [SerializeField] private Toggle _selectAllToggle;
        [SerializeField] private Button _infoButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private SwitchGradeGroupView[] _gradeSwitchers;

        [SerializeField] private Transform _settingsHolder;
        [SerializeField] private Transform _inactiveTabsHolder;
        [SerializeField] private Transform _activeTabHolder;
        [SerializeField] private VerticalLayoutGroup _settingsLayoutGroup;
        [SerializeField] private BaseViewAnimator _animator;

        private SwitchGradeGroupView _activeGradeGroup;

        public Transform SettingsHolder => _settingsHolder;
        public SwitchGradeGroupView[] GradeSwitchers => _gradeSwitchers;


        public void Show(Action onShow)
        {
            _animator.AnimateShowing(() =>
            {
                SubscribeTabsButtons();
                _closeButton.onClick.AddListener(OnCloseClick);
                onShow?.Invoke();
            });
        }

        public void Hide(Action onHide)
        {
            UnsubscribeTabsButtons();
            _closeButton.onClick.RemoveListener(OnCloseClick);
            _animator.AnimateHiding(() =>
            {
                onHide?.Invoke();
            });
        }

        public void Release()
        {
            Destroy(gameObject);
        }

        public void Init(Camera camera, int priority)
        {
            _canvas.worldCamera = camera;
            _canvas.sortingOrder = priority;
        }

        public void HandleGradeButtons(int grade)
        {
            var selected = _gradeSwitchers.FirstOrDefault(x => x.Grade == grade);
            if (_activeGradeGroup != null && _activeGradeGroup.Equals(selected))
            {
                return;
            }

            if (_activeGradeGroup != null)
            {
                var previousGroup = _activeGradeGroup;
                previousGroup.gameObject.SetActive(false);
                previousGroup.transform.SetParent(_inactiveTabsHolder);
                var index = previousGroup.Grade - 1;
                previousGroup.transform.SetSiblingIndex(index);
                previousGroup.SetActive(false);
                previousGroup.gameObject.SetActive(true);
                return;
            }

            _activeGradeGroup = selected;
            selected.gameObject.SetActive(false);
            selected.transform.SetParent(_activeTabHolder);
            selected.SetActive(true);
            selected.gameObject.SetActive(true);
        }

        private void SubscribeTabsButtons()
        {
            for (int i = 0, j = _gradeSwitchers.Length; i < j; i++)
            {
                var switcher = _gradeSwitchers[i];
                switcher.ON_GRADE_TAB_SWITCH += DoOnSwitchTabClick;
                switcher.ON_GRADE_TOGGLE_SWITCH += DoOnToggleSwitch;
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
            ON_GRADE_TAB_SWITCHED?.Invoke(index);
        }

        private void DoOnToggleSwitch(int index, bool isActive)
        {
            ON_GRADE_TOGGLE_CHANGED?.Invoke(index, isActive);
        }

        private void OnCloseClick()
        {
            ON_CLOSE_CLICK?.Invoke();
        }
    }
}
