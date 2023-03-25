using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class ShinyStarIcon : MonoBehaviour
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
    [SerializeField] private float tweenDuration = 0.75f;
    [SerializeField] private Vector3 scaleTo = new Vector3(0.25f, 0.25f, 0);
    [SerializeField] private Vector3 rotateTo = new Vector3(0, 0, 15);

    #endregion

    void Start()
    {
        Initialization();
        //ResetToDefault();
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

    public void UpdateDisplayStyle(bool isSuccessful)
    {
        if (!isTweening)
        {
            isTweening = true;
            DoTween();
        }

        image.sprite = icons[isSuccessful ? 1 : 0];

        if (isSuccessful)
        {
            fx.gameObject.SetActive(true);
            fx.Stop();
            fx.Play();
        }
        else
        {
            fx.gameObject.SetActive(false);
        }
    }

    private void DoTween()
    {
        var sequence = DOTween.Sequence();

        if (isScaling)
        {
            sequence.Join(rTransform.DOPunchScale(scaleTo, tweenDuration).SetEase(Ease.InOutQuad));
        }
        if (isRotating)
        {
            sequence.Join(rTransform.DOShakeRotation(tweenDuration, rotateTo).SetEase(Ease.InOutQuad));
        }
        sequence.OnComplete(() => OnTweenComplete());
    }

    private void OnTweenComplete()
    {
        isTweening = false;
    }
}
