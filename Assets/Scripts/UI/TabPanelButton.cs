using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;

public class TabPanelButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabGroup tabGroup;
    public Image tabImage;
    [SerializeField] protected TMP_Text title;
    protected RectTransform tabRect;
    protected bool isPressed = false;
    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;
    [SerializeField] protected List<Sprite> stateImages;

    [Header("Tween:")]
    [SerializeField] protected bool isTweened = true;
    [SerializeField] protected float tweenDuration = 0.5f;
    [SerializeField] protected float sizeTo = 175f;
    [SerializeField] protected float sizeDefault = 150f;
    [SerializeField] protected float fontSizeTo = 80f;
    [SerializeField] protected float fontSizeDefault = 60f;
    [SerializeField] protected List<Color> fontColors;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(GetComponent<Button>().interactable) tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    private void Awake()
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        if (tabImage == null) tabImage = GetComponent<Image>();
        if (title == null) title = GetComponentInChildren<TMP_Text>();
        tabRect = tabImage.GetComponent<RectTransform>();
        ResetToDefault();
        tabGroup.Subscribe(this);
    }

    protected virtual void ResetToDefault()
    {
        tabRect.sizeDelta = new Vector2(sizeDefault, tabRect.sizeDelta.y);
        title.fontSize = fontSizeDefault;
    }

    protected virtual void SelectionTween(bool isSelected)
    {
        float endHeight = isSelected ? sizeTo : sizeDefault;
        float endFontSize = isSelected ? fontSizeTo : fontSizeDefault;
        Color endFontColor = isSelected ? fontColors[1] : fontColors[0];
        float twenRatio = isSelected ? 1.1f : 0.9f;

        var sequence = DOTween.Sequence();
        sequence.Join(tabRect.DOSizeDelta(new Vector2(tabRect.sizeDelta.x, endHeight), tweenDuration).SetEase(Ease.InOutBack));
        sequence.Join(DOTween.To(() => title.fontSize, x => title.fontSize = x, endFontSize, tweenDuration).SetEase(Ease.InOutBack));
        sequence.Join(title.DOColor(endFontColor, tweenDuration));
        sequence.OnComplete(() => OnTweenComplete());
    }

    protected virtual void OnTweenComplete()
    {
        //isPressed = false;
    }

    public virtual void Select()
    {
        if (onTabSelected != null)
        {
            onTabSelected.Invoke();
        }
        tabImage.sprite = stateImages[1];
        transform.SetAsLastSibling();
        SelectionTween(true);
    }

    public virtual void Deselect()
    {
        if (onTabDeselected != null)
        {
            onTabDeselected.Invoke();
        }
        tabImage.sprite = stateImages[0];
        SelectionTween(false);
    }
}
