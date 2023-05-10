using DG.Tweening;
using Mathy.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.UI
{
    public interface ISkillResultProgressView
    {
        SkillType Skill { get; }
        void Init();
        void SetTitle(string title);
        void SetProgressBar(int target, int previous, bool isAnimated = true);
        void SetResults(string results);
        void ShowChangedValue(string value);
    }


    public class SkillResultProgressView : MonoBehaviour, ISkillResultProgressView
    {
        private const float kSliderStepDuration = 0.03f;

        [SerializeField] private Slider _progressBar;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _resultsText;
        [SerializeField] private TMP_Text _changableValueText;
        [SerializeField] private GameObject _changableHolder;

        [field: SerializeField] public SkillType Skill { get; private set; }


        public void Init()
        {
            DOTween.Kill(transform);
            _changableHolder.SetActive(false);
        }

        private void OnDisable()
        {
            DOTween.Kill(transform);
        }

        public void SetProgressBar(int target, int previous, bool isAnimated = true)
        {
            if (!isAnimated)
            {
                _progressBar.value = target;
                return;
            }
            _progressBar.value = previous;

            var maxValue = MathF.Max(target, previous);
            var minValue = MathF.Min(target, previous);
            var duration = (maxValue - minValue) * kSliderStepDuration;
            _progressBar.DOValue(target, duration).SetEase(Ease.Linear).SetId(transform);
        }

        public void SetTitle(string  title)
        {
            _title.text = title;
        }

        public void SetResults(string results)
        {
            _resultsText.text = results;
        }

        public void ShowChangedValue(string value)
        {
            _changableHolder.SetActive(true);
            _changableValueText.text = value;
        }
    }
}


