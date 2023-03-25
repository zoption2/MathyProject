using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SwitchToggle : MonoBehaviour
{
    [Header("COMPONENTS:")]
    [SerializeField] private RectTransform handleTransfrorm;
    [SerializeField] private Image toggleFillImage;
    [SerializeField] private Toggle toggle;

    [Header("TWEEN:")]
    [SerializeField] private float tweenDuration = 0.25f;
    private Vector2 handlePosition;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        handlePosition = handleTransfrorm.anchoredPosition;
        toggle.onValueChanged.AddListener(OnSwitch);
        OnSwitch(toggle.isOn);
    }

    private void OnSwitch(bool on)
    {
        var sequence = DOTween.Sequence();
        sequence.Join(handleTransfrorm.DOAnchorPos(on ? -handlePosition : handlePosition, tweenDuration));
        sequence.Join(toggleFillImage.DOFade(on ? 1 : 0, tweenDuration).SetDelay(tweenDuration * 0.5f));
        sequence.SetEase(Ease.InOutBack);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}