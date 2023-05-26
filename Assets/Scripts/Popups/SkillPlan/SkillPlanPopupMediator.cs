using Cysharp.Threading.Tasks;
using Mathy.Services;
using Mathy.Services.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mathy.UI
{
    public interface ISkillPlanPopupMediator : IPopupMediator
    {

    }


    public class SkillPlanPopupMediator : ISkillPlanPopupMediator, IPopupView
    {
        public event Action ON_CLOSE_CLICK;

        private const string kLocalizeTable = "Grades and Skills";
        private const string kGradeNameFormat = "Skills_Panel_Grade{0}";
        private const string kSelectAllKey = "Skills Panel Select All";
        private const string kDeselectAllKey = "Skills Panel Deselect All";

        private readonly IAddressableRefsHolder _refsHolder;
        private readonly IUIManager _uiManager;
        private readonly ISkillPlanService _skillPlanService;

        private SkillPlanPopupView _generalView;
        private SwitchGradeGroupView[] _switchers;
        private Dictionary<int, ISkillPlanGradeController> _controllers;
        private ISkillPlanGradeController _currentController;
        private int _currentGrade;

        private string _selectedGradeKey = KeyValueIntegerKeys.SelectedGrade.ToString();

        public SkillPlanPopupMediator(IAddressableRefsHolder refsHolder
            , IUIManager uIManager
            , ISkillPlanService skillPlanService)
        {
            _refsHolder = refsHolder;
            _uiManager = uIManager;
            _skillPlanService = skillPlanService;
        }


        public void CreatePopup(Action onComplete = null)
        {
            _uiManager.OpenView(this, viewBehaviour: UIBehaviour.StayWithNew, onShow: onComplete);
        }

        public void ClosePopup(Action onComplete = null)
        {
            _uiManager.CloseView(this, onComplete);
        }

        public async UniTask InitPopup(Camera camera, Transform parent, int orderLayer = 0)
        {
            _generalView = await _refsHolder.Popups.Main.InstantiateFromReference<SkillPlanPopupView>(Popups.SkillPlan, parent);
            _generalView.Init(camera, orderLayer);
            _currentGrade = await _skillPlanService.GetCurrentGrade();

            _currentController = null;
            _controllers = new Dictionary<int, ISkillPlanGradeController>();

            _switchers = _generalView.GradeSwitchers;
            for (int i = 0, j = _switchers.Length; i < j; i++)
            {
                var switcher = _switchers[i];
                var grade = switcher.Grade;
                var key = string.Format(kGradeNameFormat, grade);
                var localizedName = LocalizationManager.GetLocalizedString(kLocalizeTable, key);
                var isSelected = grade == _currentGrade;
                switcher.Init(localizedName, isSelected);
            }
            SubscribeSwitchers();
        }

        public void Show(Action onShow)
        {
            TryHideCurrentController();
            SelectGrade(_currentGrade);
            _generalView.Show(()=>
            {
                _generalView.ON_CLOSE_CLICK += DoOnCloseClick;
                _generalView.ON_SELECT_ALL_CLICK += DoOnSelectAllClick;
                onShow?.Invoke();
            });
        }

        public void Hide(Action onHide)
        {
            _generalView.ON_CLOSE_CLICK -= DoOnCloseClick;
            _generalView.ON_SELECT_ALL_CLICK -= DoOnSelectAllClick;
            _generalView.Hide(onHide);
        }

        public void Release()
        {
            foreach (var controller in _controllers.Values)
            {
                controller.Release();
            }

            UnsubscribeSwitchers();
            _controllers = null;
            _switchers = null;
            _currentController = null;
            _generalView.Release();
        }

        private async void SelectGrade(int grade)
        {
            UnsubscribeSwitchers();
            await InitControllerOnSelect(grade);
            _generalView.HandleGradeButtons(grade);
            _currentController.Show(()=>
            {
                SubscribeSwitchers();
            });
        }

        private async UniTask InitControllerOnSelect(int grade)
        {
            if (!_controllers.ContainsKey(grade))
            {
                var controller = GetControllerByGrade(grade);
                _controllers.Add(grade, controller);
                await controller.Init(_generalView);
            }

            _currentController = _controllers[grade];
            bool isAnySkillEnable = _currentController.IsAnySkillEnabled();
            LocalizeSelectAllText(isAnySkillEnable);
            _generalView.SetSelectAllToggle(isAnySkillEnable);
        }

        private void TryHideCurrentController()
        {
            if (_currentController != null)
            {
                _currentController.Hide(null);
                _currentController = null;
            }
        }

        private async void DoOnCloseClick()
        {
            _generalView.ON_CLOSE_CLICK -= DoOnCloseClick;
            await _skillPlanService.SetCurrentGrade(_currentGrade);
            ClosePopup();
            ON_CLOSE_CLICK?.Invoke();
        }

        private void DoOnSelectAllClick(bool isOn)
        {
            LocalizeSelectAllText(isOn);
            _currentController.SetAllSkillsActive(isOn);
        }

        private void LocalizeSelectAllText(bool isOn)
        {
            var key = isOn ? kDeselectAllKey : kSelectAllKey;
            var localizedText = LocalizationManager.GetLocalizedString(kLocalizeTable, key);
            _generalView.SetSelectAllText(localizedText);
        }

        private void DoOnGradeTabChange(int grade)
        {
            if (_currentGrade == grade)
            {
                return;
            }
            _currentGrade = grade;
            TryHideCurrentController();
            SelectGrade(_currentGrade);
        }

        private void DoOnGradeToggleSwitched(int grade, bool isOn)
        {
            if (isOn && grade != _currentGrade)
            {
                _currentGrade = grade;
                TryHideCurrentController();
                SelectGrade(grade);
            }
        }

        private ISkillPlanGradeController GetControllerByGrade(int grade)
        {
            ISkillPlanGradeController controller = null;
            switch (grade)
            {
                case 1:
                    controller = new SkillPlanFirstGradeController(_skillPlanService, _refsHolder);
                    break;

                case 2:
                    controller = new SkillPlanSecondGradeController(_skillPlanService, _refsHolder);
                    break;

                default:
                    break;
            }
            return controller;
        }

        private void SubscribeSwitchers()
        {
            for (int i = 0, j = _switchers.Length; i < j; i++)
            {
                var switcher = _switchers[i];
                switcher.ON_GRADE_TAB_SWITCH += DoOnGradeTabChange;
                switcher.ON_GRADE_TOGGLE_SWITCH += DoOnGradeToggleSwitched;
                switcher.Enable();
            }
        }

        private void UnsubscribeSwitchers()
        {
            for (int i = 0, j = _switchers.Length; i < j; i++)
            {
                var switcher = _switchers[i];
                switcher.ON_GRADE_TAB_SWITCH -= DoOnGradeTabChange;
                switcher.ON_GRADE_TOGGLE_SWITCH -= DoOnGradeToggleSwitched;
                switcher.Disable();
            }
        }
    }
}
