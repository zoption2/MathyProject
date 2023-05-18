using UnityEngine;
using System;
using DG.Tweening;

namespace Mathy.UI
{
    public class PopupScaleAnimator : BaseViewAnimator
    {
        [SerializeField] private float appearTime = 0.5f;
        [SerializeField] private float hideTime = 0.5f;
        [SerializeField] private Ease appearEase = Ease.OutBack;
        [SerializeField] private Ease hidingEase = Ease.OutBack;
        [SerializeField, Range(0f, 1f)] private float startSizeCoef = 0.75f;
        [SerializeField] private Vector3 showedScale = Vector3.one;
        [SerializeField] private Vector3 hidedScale = Vector3.zero;
        [SerializeField] private Transform animated;


        public override void AnimateShowing(Action onComplete)
        {
            animated.localScale = showedScale * startSizeCoef;
            animated.DOScale(showedScale, appearTime).SetEase(appearEase).SetId(transform).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public override void AnimateHiding(Action onComplete)
        {
            animated.DOScale(hidedScale, hideTime).SetEase(hidingEase).SetId(transform).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
    }
}

