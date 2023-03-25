using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAppPurchaseDebugPanel : MonoBehaviour
{
    [SerializeField] private RectTransform rTransform;
    private bool isOpened = false;
    public bool IsOpened
    {
        get
        {
            return isOpened;
        }
        set
        {
            isOpened = value;
            if (isOpened) OpenPanel();
            else ClosePanel();
        }
    }

    public void ShowPanel()
    {
        IsOpened = !IsOpened;
    }

    public void OpenPanel()
    {
        rTransform.DOAnchorPosX(0, 0.25f).SetEase(Ease.InOutQuad);
    }

    public void ClosePanel()
    {
        rTransform.DOAnchorPosX(rTransform.rect.width, 0.2f).SetEase(Ease.InOutQuad);
    }
}
