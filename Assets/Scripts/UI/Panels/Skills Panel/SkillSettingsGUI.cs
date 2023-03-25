using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mathy.UI
{
    public class SkillSettingsGUI : MonoBehaviour
    {
        #region FIELDS

        [Header("COMPONENTS:")]
        [SerializeField] private Toggle isActiveToggle;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private Button detailsButton;
        [SerializeField] private Slider maxNumberSlider;
        [SerializeField] private Image sliderBaseImage;

        [Header("REFERENCES:")]
        [SerializeField] private List<Sprite> sliderStatusImages;

        [Header("CONFIG:")]
        [SerializeField] private int maxNumber;
        public int MaxNumber
        {
            get => maxNumber;
            private set
            {
                maxNumber = value;
                maxNumberSlider.value = maxNumber / maxNumberSlider.maxValue;
            }
        }

        [SerializeField] private bool isActive;
        public bool IsActive
        {
            get => isActive;
            private set
            {
                isActive = value;
                UpdateSliderState();
                isActiveToggle.isOn = isActive;
            }
        }

        [Header("EVENTS:")]
        public UnityEvent<bool> OnTogglePressed;
        public UnityEvent<int> OnSliderValueChanged;

        #endregion

        private void Start()
        {
            isActiveToggle.onValueChanged.AddListener(delegate { 
                SetActive(isActiveToggle.isOn); });
            maxNumberSlider.onValueChanged.AddListener(delegate { 
                SetMaxNumber(maxNumberSlider.value * maxNumberSlider.maxValue); });
        }

        public void SetActive(bool isSkillActive)
        {
            IsActive = isSkillActive;
            if (OnTogglePressed != null) OnTogglePressed.Invoke(IsActive);
        }

        public void SetMaxNumber(float skillMaxNumber)
        {
            maxNumber = (int)skillMaxNumber;
            if (OnSliderValueChanged != null) OnSliderValueChanged.Invoke(MaxNumber);
        }

        private void UpdateSliderState()
        {
            if (isActive)
            {
                maxNumberSlider.interactable = true;
                maxNumberSlider.fillRect.gameObject.SetActive(true);
                maxNumberSlider.handleRect.gameObject.SetActive(true);
                sliderBaseImage.sprite = sliderStatusImages[0];
            }
            else
            {
                maxNumberSlider.interactable = false;
                maxNumberSlider.fillRect.gameObject.SetActive(false);
                maxNumberSlider.handleRect.gameObject.SetActive(false);
                sliderBaseImage.sprite = sliderStatusImages[1];
            }
            
        }
        public void Initialize(string nameLabel, int maxNumber, bool isActive)
        {
            this.nameLabel.text = nameLabel;
            this.maxNumberSlider.value = maxNumber / 10;
            this.IsActive = isActive;
        }

        public void Localize(string nameLabel)
        {
            this.nameLabel.text = nameLabel;
        }
    }
}