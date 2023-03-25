using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockIcon : ButtonFX
{
    #region Fields



    #endregion

    void Start()
    {
        SwayingTween();
    }

    private void SwayingTween()
    {
        var sequence = DOTween.Sequence();
        sequence.Join(rotatingElement.DORotate(new Vector3(0, 0, 20), 2f).SetEase(Ease.InOutQuad));
        sequence.Append(rotatingElement.DORotate(new Vector3(0, 0, -20), 2f).SetEase(Ease.InOutQuad));
        sequence.OnComplete(() => SwayingTween());
    }
}
