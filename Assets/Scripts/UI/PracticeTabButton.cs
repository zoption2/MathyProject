using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeTabButton : TabPanelButton
{
    protected override void Initialization()
    {
        tabRect = tabImage.GetComponent<RectTransform>();
        ResetToDefault();
        tabGroup.Subscribe(this);
    }

    protected override void SelectionTween(bool isSelected)
    {
        float endwidth = isSelected ? sizeTo : sizeDefault;
        float endFontSize = isSelected ? fontSizeTo : fontSizeDefault;
        float twenRatio = isSelected ? 1.1f : 0.9f;

        var sequence = DOTween.Sequence().SetEase(Ease.InOutBack).OnComplete(() => OnTweenComplete());
        sequence.Join(tabRect.DOSizeDelta(new Vector2(endwidth, tabRect.sizeDelta.y), tweenDuration));
        sequence.Join(DOTween.To(() => title.fontSize, x => title.fontSize = x, endFontSize, tweenDuration));
    }

    public override void Select()
    {
        if (onTabSelected != null) onTabSelected.Invoke();
        if (isTweened) SelectionTween(true);
    }

    public override void Deselect()
    {
        if (onTabDeselected != null) onTabDeselected.Invoke();
        if (isTweened) SelectionTween(false);
    }
}
