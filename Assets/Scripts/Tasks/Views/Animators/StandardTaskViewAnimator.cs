using UnityEngine;
using System;
using DG.Tweening;

namespace Mathy.UI
{
    public class StandardTaskViewAnimator : BaseViewAnimator
    {
        private const float kFadeTime = .5f;

        [SerializeField] private CanvasGroup canvasGroup;

        public override void AnimateShowing(Action onComplete)
        {
            onComplete?.Invoke();
        }

        public override void AnimateHiding(Action onComplete)
        {
            canvasGroup.DOFade(0, kFadeTime).SetEase(Ease.Linear).SetId(transform).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
    }
}

