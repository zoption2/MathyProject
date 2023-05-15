using System;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Mathy.UI
{
    public interface IResultScreenRewardView : IView
    {
        void SetTitle(string title);
        void SetExperience(int result, int previous, bool isAnimated);
    }


    public class ResultScreenRewardView : MonoBehaviour, IResultScreenRewardView
    {
        private const float kAnimationTime = 1f;
        private const string kAdditionatValueFormat = "+{0}";

        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _value;
        [SerializeField] private TMP_Text _additionalValue;
        [SerializeField] private GameObject _additionalPart;

        public void SetTitle(string title)
        {
            _title.text = title;
        }

        public void Show(Action onShow)
        {
            gameObject.SetActive(true);
            onShow?.Invoke();
        }

        public void Hide(Action onHide)
        {
            DOTween.Kill(transform);
        }

        public void Release()
        {
            
        }

        public void SetExperience(int result, int previous, bool isAnimated)
        {
            var difference = result - previous;
            _additionalPart.SetActive(difference > 0);
            _additionalValue.text = string.Format(kAdditionatValueFormat, difference);

            if (!isAnimated)
            {
                _value.text = result.ToString();
                return;
            }

            _value.text = previous.ToString();
            DOVirtual.Int(previous, result, kAnimationTime, UpdateValue).SetEase(Ease.Linear).SetId(transform);
        }

        private void UpdateValue(int value)
        {
            _value.text = value.ToString();
        }
    }
}


