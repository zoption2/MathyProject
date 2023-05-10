using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.UI
{
    public interface IResultScreenAchievementsView : IView
    {
        void SetTitle(string title);
        IAchievementView[] AchievementViews { get; }
    }


    public class ResultScreenAchievementsView : MonoBehaviour, IResultScreenAchievementsView
    {
        private const int kAchivementsScrollOffset = 3;

        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;
        [SerializeField] private AchievementView[] _achievementViews;
        [SerializeField] private Scrollbar _scrollbar;

        private float _stepValue;
        private int _totalSteps;
        private int _currentStep;

        public IAchievementView[] AchievementViews => _achievementViews;


        public void Show(Action onShow)
        {
            _leftButton.onClick.AddListener(OnLeftClick);
            _rightButton.onClick.AddListener(OnRightClick);
            InitScrolling();
            SetupButtons();
            gameObject.SetActive(true);
            onShow?.Invoke();
        }

        public void Hide(Action onHide)
        {
            _leftButton.onClick.RemoveListener(OnLeftClick);
            _rightButton.onClick.RemoveListener(OnRightClick);
            gameObject.SetActive(false);
            onHide?.Invoke();
        }

        public void Release()
        {
            
        }

        public void SetTitle(string title)
        {
            _titleText.text = title;
        }

        private void OnLeftClick()
        {
            _currentStep -= 1;
            SetupButtons();
            SetupScrolling();
        }

        private void OnRightClick()
        {
            _currentStep += 1;
            SetupButtons();
            SetupScrolling();
        }

        private void SetupButtons()
        {
            _leftButton.interactable = _currentStep > 1;
            _rightButton.interactable = _currentStep < _totalSteps;
        }

        private void SetupScrolling()
        {
            var value = (_currentStep - 1) * _stepValue;
            _scrollbar.value = value;
        }

        private void InitScrolling()
        {
            var achivementsCount = _achievementViews.Length;
            _totalSteps = achivementsCount - kAchivementsScrollOffset;
            _currentStep = 1;
            _stepValue = 1 / (float)(_totalSteps - 1);
            _scrollbar.numberOfSteps = _totalSteps;
            _scrollbar.value = 0;
        }
    }


    public class ResultScreenAchievementsModel : IModel
    {
        public string LocalizedTitle { get; set; }
        public Dictionary<Achievements, AchievementModel> Achievements { get; set; }
    }

    public class AchievementModel 
    {
        public Achievements Achievement { get; set; }
        public int Value { get; set; }
    }
}


