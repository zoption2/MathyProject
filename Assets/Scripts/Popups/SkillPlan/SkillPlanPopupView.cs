using System;
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
        public event Action<bool> ON_SELECT_ALL_CLICK;

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
                _closeButton.onClick.AddListener(OnCloseClick);
                _selectAllToggle.onValueChanged.AddListener(OnSelectAllClick);
                onShow?.Invoke();
            });
        }

        public void Hide(Action onHide)
        {
            _closeButton.onClick.RemoveListener(OnCloseClick);
            _selectAllToggle.onValueChanged.RemoveListener(OnSelectAllClick);
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
                previousGroup.SetActive(false);
                previousGroup.gameObject.SetActive(true);
            }

            _activeGradeGroup = selected;
            selected.gameObject.SetActive(false);
            selected.transform.SetParent(_activeTabHolder);
            selected.SetActive(true);
            selected.gameObject.SetActive(true);
        }

        public void SetSelectAllText(string text)
        {
            _selectAllText.text = text;
        }

        public void SetSelectAllToggle(bool isOn)
        {
            _selectAllToggle.isOn = isOn;
        }

        private void OnSelectAllClick(bool isOn)
        {
            ON_SELECT_ALL_CLICK?.Invoke(isOn);
        }

        private void OnCloseClick()
        {
            ON_CLOSE_CLICK?.Invoke();
        }
    }
}
