using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TweenedProgressBar : MonoBehaviour
{
    #region FIELDS

    [Header("COMPONENTS:")]
    [SerializeField] protected Slider progressBar;
    [SerializeField] protected Transform icon;

    [Header("TWEEN:")]
    [SerializeField] protected float tweenDuration;

    protected float sliderValue;
    public float Value
    {
        get
        {
            return sliderValue;
        }
        set
        {
            sliderValue = value / 100;
            if (progressBar.value != sliderValue) AnimateProgressBar();
        }
    }

    #endregion

    private void OnEnable()
    {
        if (progressBar.value != sliderValue) AnimateProgressBar();
    }

    private void AnimateProgressBar()
    {
        var sequence = DOTween.Sequence();
        sequence.Join(progressBar.DOValue(sliderValue, tweenDuration).SetEase(Ease.InOutQuad));
        sequence.Append(icon.DOPunchScale(new Vector2(0.25f, -0.1f), 1f).SetEase(Ease.InOutQuad))
            .OnComplete(OnTweenCompleted);
    }

    private void OnTweenCompleted()
    {
        icon.localScale = Vector2.one;
        progressBar.value = sliderValue;
    }
}
