using Mathy.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.UI
{
    public interface ISkillPlanSettingAdapter
    {
        event Action<bool> ON_TOGGLE_CLICK;
        event Action<int> ON_VALUE_CHANGE;
        event Action ON_INFO_CLICK;
        void Init(bool isActive, int value);
        void SetTitle(string title);
        int GetMinValue();
        int GetMaxValue();
        void Release();
        int Value { get; }
        bool IsEnable { get; }
        SkillType SkillType { get; }
    }


    public abstract class BaseSkillSetting : MonoBehaviour, ISkillPlanSettingAdapter
    {
        public event Action<bool> ON_TOGGLE_CLICK;
        public event Action<int> ON_VALUE_CHANGE;
        public event Action ON_INFO_CLICK;

        [SerializeField] protected SkillType _skillType;
        [SerializeField] protected Slider _valueSlider;
        [SerializeField] private Toggle _isActiveToggle;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Button _infoButton;
        private int _previousValue;
        private bool _previousIsActive;

        public int Value => Convert.ToInt32(_valueSlider.value);
        public bool IsEnable => _isActiveToggle.isOn;
        public SkillType SkillType => _skillType;


        public void Init(bool isActive, int value)
        {
            _valueSlider.value = value;
            _isActiveToggle.isOn = isActive;
            _previousIsActive = isActive;
            _previousValue = value;

            _valueSlider.onValueChanged.AddListener(SetValue);
            _isActiveToggle.onValueChanged.AddListener(SetActive);
            _infoButton.onClick.AddListener(DoOnInfoClick);
            DoOnInit();
        }

        public void SetTitle(string title)
        {
            _title.text = title;
        }

        public int GetMinValue()
        {
            var integerValue = Convert.ToInt32(_valueSlider.minValue);
            return integerValue;
        }

        public int GetMaxValue()
        {
            var integerValue = Convert.ToInt32(_valueSlider.maxValue);
            return integerValue;
        }

        public void Release()
        {
            _valueSlider.onValueChanged.RemoveListener(SetValue);
            _isActiveToggle.onValueChanged.RemoveListener(SetActive);
            _infoButton.onClick.RemoveListener(DoOnInfoClick);
            DoOnRelease();
        }

        private void SetActive(bool isActive)
        {
            if (_previousIsActive != isActive)
            {
                _previousIsActive = isActive;
                ON_TOGGLE_CLICK?.Invoke(isActive);
                DoOnToggleChanged(isActive);
            }
        }

        private void SetValue(float value)
        {
            var integerValue = Convert.ToInt32(value);
            if (_previousValue != integerValue)
            {
                _previousValue = integerValue;
                ON_VALUE_CHANGE?.Invoke(integerValue);
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
    }
}


