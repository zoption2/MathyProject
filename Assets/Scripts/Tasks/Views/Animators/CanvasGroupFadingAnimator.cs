using UnityEngine;
using System;
using DG.Tweening;

namespace Mathy.UI
{
    public class CanvasGroupFadingAnimator : BaseViewAnimator
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField, Range(0f, 1f)] private float startOpaqueValue = 0;
        [SerializeField, Range(0f, 1f)] private float endOpaqueValue = 0.6f;
        [SerializeField] private float appearTime = 0.5f;
        [SerializeField] private float fadeTime = 0.3f;


        public override void AnimateShowing(Action onComplete)
        {
            canvasGroup.alpha = startOpaqueValue;
            canvasGroup.DOFade(endOpaqueValue, appearTime).SetEase(Ease.Linear).SetId(transform).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public override void AnimateHiding(Action onComplete)
        {
            canvasGroup.DOFade(0, fadeTime).SetEase(Ease.Linear).SetId(transform).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
    }
}

