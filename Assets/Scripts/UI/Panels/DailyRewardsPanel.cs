using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DailyRewardsPanel : MonoBehaviour
{
    [Header("COMPONENTS:")]
    [SerializeField] private Transform popupPanel;
    [SerializeField] private CanvasGroup background;

    [Header("TWEEN:")]
    [SerializeField] private Vector2 scaleTo = new Vector2(0.1f, 0.1f);
    [SerializeField] private float tweenDuration = 0.25f;


    private void OnEnable()
    {
        OpenPanel();
    }

    public void OpenPanel()
    {
        background.alpha = 0;
        popupPanel.localScale = Vector3.zero;

        var sequence = DOTween.Sequence();        
        sequence.Join(background.DOFade(1, tweenDuration));
        sequence.Append(popupPanel.DOScale(Vector3.one, tweenDuration));
        sequence.Append(popupPanel.DOPunchScale(scaleTo, tweenDuration));
        sequence.SetEase(Ease.InOutQuad);
    }

    public void ClosePanel()
    {
        var sequence = DOTween.Sequence();        
        sequence.Join(popupPanel.DOScale(Vector3.zero, tweenDuration));
        sequence.Append(background.DOFade(0, tweenDuration));
        sequence.SetEase(Ease.InOutQuad).OnComplete(() => gameObject.SetActive(false));
    }
}