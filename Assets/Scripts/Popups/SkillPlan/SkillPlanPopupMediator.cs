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

        private readonly IAddressableRefsHolder _refsHolder;
        private readonly IUIManager _uiManager;
        private readonly ISkillPlanFirstGradeController _firstGradeController;
        private readonly ISkillPlanSecondGradeController _secondGradeController;
        private readonly ISkillPlanService _skillPlanService;

        private SkillPlanPopupView _generalView;
        private List<ISkillPlanGradeController> _controllers;
        private ISkillPlanGradeController _currentController;
        private int _currentGrade;

        private string _selectedGradeKey = KeyValueIntegerKeys.SelectedGrade.ToString();

        public SkillPlanPopupMediator(IAddressableRefsHolder refsHolder
            , IUIManager uIManager
            , ISkillPlanFirstGradeController firstGradeController
            , ISkillPlanSecondGradeController secondGradeController
            , ISkillPlanService skillPlanService)
        {
            _refsHolder = refsHolder;
            _uiManager = uIManager;
            _firstGradeController = firstGradeController;
            _secondGradeController = secondGradeController;
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

            _controllers = new List<ISkillPlanGradeController>()
            {
                _firstGradeController,
                _secondGradeController
            };

            var switchers = _generalView.GradeSwitchers;
            for (int i = 0, j = switchers.Length; i < j; i++)
            {
                var switcher = switchers[i];
                var grade = switcher.Grade;
                var key = string.Format(kGradeNameFormat, grade);
                var localizedName = LocalizationManager.GetLocalizedString(kLocalizeTable, key);
                var isSelected = grade == _currentGrade;
                switcher.Init(localizedName, isSelected);
                switcher.Enable();
            }
        }

        public void Show(Action onShow)
        {
            TryHideCurrentController();
            SelectGrade(_currentGrade);
            _generalView.Show(()=>
            {
                _generalView.ON_GRADE_TAB_SWITCHED += DoOnGradeTabChange;
                _generalView.ON_GRADE_TOGGLE_CHANGED += DoOnGradeToggleSwitched;
                _generalView.ON_CLOSE_CLICK += DoOnCloseClick;
                onShow?.Invoke();
            });
        }

        public void Hide(Action onHide)
        {
            _generalView.ON_GRADE_TAB_SWITCHED -= DoOnGradeTabChange;
            _generalView.ON_GRADE_TOGGLE_CHANGED -= DoOnGradeToggleSwitched;
            _generalView.ON_CLOSE_CLICK -= DoOnCloseClick;
            _generalView.Hide(onHide);
        }

        public void Release()
        {
            _generalView.Hide(() =>
            {
                _firstGradeController.Release();
                _generalView.Release();
            });
        }

        private void SelectGrade(int grade)
        {
            InitControllerOnSelect(grade);
            _generalView.HandleGradeButtons(grade);
            _currentController.Show(null);
        }

        private void InitControllerOnSelect(int grade)
        {
            int index = grade - 1;
            var controller = _controllers[index];
            controller.Init(_generalView);
            _currentController = controller;
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

        private void ShowGradeTab(int grade)
        {
            for (int i = 0, j = _controllers.Count; i < j; i++)
            {
                if (_controllers[i].Grade.Equals(grade))
                {
                    _controllers[i].Show(null);
                }
                else
                {
                    _controllers[i].Hide(null);
                }
            }
        }
    }
}
