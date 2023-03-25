using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorTabButton : TabPanelButton
{
    protected override void Initialization()
    {
        tabRect = tabImage.GetComponent<RectTransform>();
        ResetToDefault();
        tabGroup.Subscribe(this);
    }

    protected override void ResetToDefault()
    {
        if (isTweened) tabRect.sizeDelta = new Vector2(tabRect.sizeDelta.x, sizeDefault);
    }

    protected override void SelectionTween(bool isSelected)
    {
        float endHeight = isSelected ? sizeTo : sizeDefault;
        float endFontSize = isSelected ? fontSizeTo : fontSizeDefault;
        float twenRatio = isSelected ? 1.1f : 0.9f;

        var sequence = DOTween.Sequence();
        sequence.Join(tabRect.DOSizeDelta(new Vector2(tabRect.sizeDelta.x, endHeight), tweenDuration).SetEase(Ease.InOutBack));
        sequence.OnComplete(() => OnTweenComplete());
    }

    protected override void OnTweenComplete()
    {
        
    }

    public override void Select()
    {
        if (onTabSelected != null)
        {
            onTabSelected.Invoke();
        }
        tabImage.sprite = stateImages[1];
        //transform.SetAsLastSibling();
        if (isTweened) SelectionTween(true);
    }

    public override void Deselect()
    {
        if (onTabDeselected != null)
        {
            onTabDeselected.Invoke();
        }
        tabImage.sprite = stateImages[0];
        if (isTweened) SelectionTween(false);
    }
}
