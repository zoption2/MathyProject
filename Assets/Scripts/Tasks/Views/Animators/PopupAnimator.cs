using UnityEngine;
using System;
using DG.Tweening;

namespace Mathy.UI
{
    public class PopupAnimator : BaseViewAnimator
    {
        [SerializeField] private float appearTime = 0.5f;
        [SerializeField] private float fadeTime = 0.3f;
        [SerializeField] private Ease appearEase = Ease.OutBack;
        [SerializeField, Range(0f, 1f)] private float startSize = 0.75f;
        [SerializeField] private Vector3 endScale = Vector3.one;
        [SerializeField] private Transform animated;
        [SerializeField] private CanvasGroup canvasGroup;

        public override void AnimateShowing(Action onComplete)
        {
            animated.localScale = endScale * startSize;
            animated.DOScale(endScale, appearTime).SetEase(appearEase).SetId(transform).OnComplete(() =>
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

