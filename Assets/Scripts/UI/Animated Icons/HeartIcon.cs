using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class HeartIcon : MonoBehaviour
{
    #region FIELDS

    private bool isTweening = false;
    private RectTransform rTransform;

    [Header("Components:")]
    [SerializeField] protected Image image;
    [SerializeField] protected ParticleSystem fx;
    [SerializeField] protected List<Sprite> icons;

    [Header("Tween:")]
    [SerializeField] private bool isRotating = false;
    [SerializeField] private bool isScaling = true;
    [SerializeField] private float tweenDuration = 0.25f;
    [SerializeField] private Vector3 scaleTo = new Vector3(0.1f, 0.5f, 0);
    [SerializeField] private Vector3 rotateTo = new Vector3(0, 0, 15);

    #endregion

    void Start()
    {
        Initialization();
        ResetToDefault();
    }

    private void OnDisable()
    {
        ResetToDefault();
    }

    public void ResetToDefault()
    {
        isTweening = false;
        image.sprite = icons[1];
    }

    private void Initialization()
    {
        if (image == null)
        {
            image = GetComponentInChildren<Image>();
        }

        rTransform = GetComponent<RectTransform>();
    }

    public void TweenHeart(bool isDamage)
    {
        if (!isTweening)
        {
            isTweening = true;
            DoTween(isDamage);
        }
    }

    private void DoTween(bool isDamage)
    {
        var sequence = DOTween.Sequence();

        if (isScaling)
        {
            sequence.Join(rTransform.DOShakeScale(tweenDuration, scaleTo, 10, 60f));
        }
        if (isRotating)
        {
            sequence.Join(rTransform.DOShakeRotation(tweenDuration, rotateTo, 40, 60).SetEase(Ease.InOutQuad));
        }
        if (isDamage)
        {
            sequence.Append(image.DOCrossfadeImage(icons[0], tweenDuration));
            fx.Stop();
            fx.Play();
        }
        else
        {
            sequence.Append(image.DOCrossfadeImage(icons[1], tweenDuration));
        }
        sequence.OnComplete(() => OnTweenComplete());
    }

    private void OnTweenComplete()
    {
        isTweening = false;
    }
}
