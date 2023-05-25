using DG.Tweening;
using Mathy.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.UI
{
    public interface ISkillPlanSettingAdapter : IView
    {
        event Action<SkillType, bool> ON_TOGGLE_CLICK;
        event Action<SkillType, int> ON_VALUE_CHANGE;
        event Action ON_INFO_CLICK;
        void Init(SkillType skillType, bool isActive, int value);
        void Enable(bool isEnabled);
        void SetTitle(string title);
        int GetMinValue();
        int GetMaxValue();
        int Value { get; }
        bool IsEnable { get; }
        SkillType SkillType { get; }
    }

    public abstract class BaseSkillSettingView : MonoBehaviour, ISkillPlanSettingAdapter
    {
        private const int kValueMultiplier = 10;

        public event Action<SkillType, bool> ON_TOGGLE_CLICK;
        public event Action<SkillType, int> ON_VALUE_CHANGE;
        public event Action ON_INFO_CLICK;
        
        [SerializeField] protected Slider _valueSlider;
        [SerializeField] private Toggle _isActiveToggle;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Button _infoButton;
        protected SkillType _skillType;
        private int _previousValue;
        private bool _previousIsActive;

        public int Value => Convert.ToInt32(_valueSlider.value) * kValueMultiplier;
        public bool IsEnable => _isActiveToggle.isOn;
        public SkillType SkillType => _skillType;


        public void Init(SkillType skillType, bool isActive, int value)
        {
            gameObject.SetActive(false);
            transform.localScale = Vector3.zero;

            _skillType = skillType;
            _valueSlider.value = value;
            _isActiveToggle.isOn = isActive;
            _previousIsActive = isActive;
            _previousValue = value;

            DoOnToggleChanged(isActive);
            DoOnInit();
        }

        public void Show(Action onShow)
        {
            gameObject.SetActive(true);
            Animate(() =>
            {
                _valueSlider.onValueChanged.AddListener(SetValue);
                _isActiveToggle.onValueChanged.AddListener(SetActive);
                _infoButton.onClick.AddListener(DoOnInfoClick);
                onShow?.Invoke();
            });
        }

        public void Hide(Action onHide)
        {
            gameObject.SetActive(false);
            _valueSlider.onValueChanged.RemoveListener(SetValue);
            _isActiveToggle.onValueChanged.RemoveListener(SetActive);
            _infoButton.onClick.RemoveListener(DoOnInfoClick);

            transform.localScale = Vector3.zero;
            onHide?.Invoke();
        }

        public void Enable(bool isEnabled)
        {
            _isActiveToggle.isOn = isEnabled;
        }

        public void SetTitle(string title)
        {
            _title.text = title;
        }

        public int GetMinValue()
        {
            var integerValue = Convert.ToInt32(_valueSlider.minValue) * kValueMultiplier;
            return integerValue;
        }

        public int GetMaxValue()
        {
            var integerValue = Convert.ToInt32(_valueSlider.maxValue) * kValueMultiplier;
            return integerValue;
        }

        public void Release()
        {

            DoOnRelease();
        }

        private void SetActive(bool isActive)
        {
            if (_previousIsActive != isActive)
            {
                _previousIsActive = isActive;
                ON_TOGGLE_CLICK?.Invoke(_skillType, isActive);
                DoOnToggleChanged(isActive);
            }
        }

        private void SetValue(float value)
        {
            var integerValue = Convert.ToInt32(value);
            if (_previousValue != integerValue)
            {
                _previousValue = integerValue;
                ON_VALUE_CHANGE?.Invoke(_skillType, integerValue);
                DoOnValueChanged(integerValue);
            }
        }

        private void DoOnInfoClick()
        {
            ON_INFO_CLICK?.Invoke();
        }

        protected virtual void DoOnInit()
        {
        }

        protected virtual void DoOnRelease()
        {
        }

        protected virtual void DoOnToggleChanged(bool isActive)
        {
            _valueSlider.interactable = isActive;
            _valueSlider.fillRect.gameObject.SetActive(isActive);
            _valueSlider.handleRect.gameObject.SetActive(isActive);
        }

        protected virtual void DoOnValueChanged(int value)
        {
        }

        private void Animate(Action onComplete)
        {
            DOTween.Kill(transform);
            transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).SetId(transform).OnComplete(()=>
                {
                    onComplete?.Invoke();
                });
        }
    }
}


