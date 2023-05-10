using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.UI;

namespace Mathy.UI
{
    public class StandardTaskViewAnimator : BaseViewAnimator
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeTime = 0.5f;

        public override void AnimateShowing(Action onComplete)
        {
            onComplete?.Invoke();
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

