using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Events;

public class PopupPanel : MonoBehaviour
{
    #region FIELDS

    [Header("GUI Components:")]

    [SerializeField] protected Transform popupPanel;
    [SerializeField] protected CanvasGroup background;

    [Header("Tween:")]

    [SerializeField] protected Vector3 scaleTo = new Vector3(0.05f, 0.025f, 0);
    [SerializeField] protected float tweenDuration = 0.2f;

    public UnityEvent OnPanelOpened = new UnityEvent();
    public UnityEvent OnPanelClosed = new UnityEvent();

    #endregion

    public virtual void Initialization()
    {
        gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        OpenPanel();
    }

    public virtual void OpenPanel()
    {
        if (background != null)
        {
            background.alpha = 0;
            background.DOFade(1, tweenDuration);
        }

        popupPanel.localScale = Vector3.zero;
        popupPanel.DOScale(Vector3.one, tweenDuration).OnComplete(() => OnComplete(true));
    }

    public virtual void ClosePanel()
    {
        if (background != null) background.DOFade(0, tweenDuration);
        popupPanel.DOScale(Vector3.zero, tweenDuration).OnComplete(() => OnComplete(false));
    }

    protected virtual void OnComplete(bool isOpened)
    {
        if (isOpened)
        {
            popupPanel.DOShakeScale(tweenDuration, scaleTo, 5, 30);
            if (OnPanelOpened != null) OnPanelOpened.Invoke();
        }
        else
        {
            gameObject.SetActive(false);
            if(OnPanelClosed != null) OnPanelClosed.Invoke();
        }
    }
}
