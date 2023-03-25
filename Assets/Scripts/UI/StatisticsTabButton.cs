using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsTabButton : TabPanelButton
{
    [SerializeField] Image icon;
    [SerializeField] protected List<Sprite> icons;

    protected override void Initialization()
    {
        tabRect = tabImage.GetComponent<RectTransform>();
        ResetToDefault();
        tabGroup.Subscribe(this);
    }

    protected override void SelectionTween(bool isSelected)
    {
        Color endFontColor;
        float endFontSize;
        if (isSelected)
        {
            endFontColor = fontColors[0];
            endFontSize = fontSizeTo;
        }
        else
        {
            endFontColor = fontColors[1];
            endFontSize = fontSizeDefault;
        }
        
        var sequence = DOTween.Sequence();
        sequence.Join(tabRect.DOPunchScale(Vector3.one * sizeTo, 
            tweenDuration).SetEase(Ease.InOutBack).
            OnComplete(() => tabRect.localScale = Vector3.one));
        
        title.fontSize = endFontSize;
        title.color = endFontColor;
    }

    public override void Select()
    {
        if (onTabSelected != null)
        {
            onTabSelected.Invoke();
        }
        tabImage.sprite = stateImages[1];
        icon.sprite = icons[1];
        transform.SetAsLastSibling();
        if (isTweened) SelectionTween(true);
    }

    public override void Deselect()
    {
        if (onTabDeselected != null)
        {
            onTabDeselected.Invoke();
        }
        tabImage.sprite = stateImages[0];
        icon.sprite = icons[0];
        if (isTweened) SelectionTween(false);
    }
}
