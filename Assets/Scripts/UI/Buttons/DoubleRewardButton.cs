using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleRewardButton : ToolbarButton
{
    #region Fields

    [SerializeField] private Transform adsIcon;
    private Sequence swayingTween;

    #endregion

    void Start()
    {
        SwayingTween();
    }

    private void SwayingTween()
    {
        adsIcon.rotation = Quaternion.Euler(0, 0, -10);
        if (swayingTween != null)
        {
            swayingTween.Restart();
            //Debug.Log("Swaying Tween has been restarted");
        }
        else
        {
            swayingTween = DOTween.Sequence();
            swayingTween.Join(adsIcon.DORotate(new Vector3(0, 0, 10), 1f).SetEase(Ease.InOutQuad));
            swayingTween.Append(adsIcon.DORotate(new Vector3(0, 0, -10), 1f).SetEase(Ease.InOutQuad));
            swayingTween.OnComplete(() => SwayingTween());
        }
    }
}
