using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSpin : MonoBehaviour
{
    private void OnEnable()
    {
        //TweenLoadingIcon();
    }

    private void TweenLoadingIcon()
    {
        var sequence = DOTween.Sequence();
        sequence.Join(transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear));
        sequence.OnComplete(() => OnTweenComplete());
    }

    private void OnTweenComplete()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        TweenLoadingIcon();
    }
}
