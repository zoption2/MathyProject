using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;

public abstract class TaskElementOLD : MonoBehaviour
{
    #region Fields

    [Header("Components:")]
    [SerializeField] protected TextMeshProUGUI textLable;
    [SerializeField] protected Image image;

    [Header("References:")]
    [SerializeField] protected Sprite answerImage;
    [SerializeField] protected TMP_FontAsset operatorFont;

    [Header("Text Tween:")]
    [SerializeField] protected bool isTextRotating = false;
    [SerializeField] protected Vector3 textRotateTo = new Vector3(20, 60, 0);

    public virtual string value { get; protected set; }
    public virtual int index { get; protected set; }

    protected RectTransform rTransform;
    protected RectTransform textTransform;
    protected RectTransform rectImage;

    #endregion

    #region Init Methods

    private void Awake()
    {
        Initialize();
    }

    public void SetIndex(int newIndex)
    {
        index = newIndex;
    }

    private void Initialize()
    {
        if (textLable == null)
        {
            textLable = GetComponentInChildren<TextMeshProUGUI>();
            textLable.text = value.ToString();
        }
        if (image == null)
        {
            image = GetComponentInChildren<Image>();
        }
        rTransform = GetComponent<RectTransform>();
        textTransform = textLable.GetComponent<RectTransform>();
        rectImage = image.transform as RectTransform;
    }

    public float fontSize()
    {
        return textLable.fontSize;
    }

    public Vector2 imageSize()
    {
        return image.rectTransform.rect.size;
    }

    public void SetImageAnchor(Vector2 newAnchor)
    {
        rectImage.anchorMin = newAnchor;
        rectImage.anchorMax = newAnchor;
        rectImage.pivot = new Vector2(0.5f, 0.5f);
        rectImage.sizeDelta = rTransform.rect.size;
    }

    public void SetImageAnchor()
    {
        rectImage.anchorMin = Vector2.zero;
        rectImage.anchorMax = Vector2.one;
        rectImage.pivot = new Vector2(0.5f, 0.5f);
        rectImage.sizeDelta = Vector2.zero;
    }

    protected virtual void ValueUpdate()
    {
        value = textLable.text;
    }

    public void SetValue(int newValue)
    {
        value = newValue.ToString();
    }

    public virtual int GetValueInt()
    {
        int parseValue;
        int.TryParse(value, out parseValue);
        return parseValue;
    }

    protected virtual void ValueUpdate(string newValue)
    {
        value = newValue;
    }

    #endregion

    #region Text Methods

    public virtual void SetText(int number)
    {
        textLable.text = number.ToString();
        ValueUpdate();
    }

    public virtual void SetText(string text)
    {
        textLable.text = text;
        SetTextOffsets();
        ValueUpdate();
    }

    public virtual void SetText(string text, string newValue)
    {
        textLable.text = text;
        SetTextOffsets();
        ValueUpdate(newValue);
    }

    protected void SetTextOffsets()
    {
        Vector4 margin;
        switch (textLable.text)
        {
            case "X":
                margin = new Vector4(0, 20, 0, 20);
                break;
            case ":":
                margin = new Vector4(0, -20, 0, 20);
                break;
            case "=":
                margin = new Vector4(0, -5, 0, 5);
                break;
            case "+":
                margin = new Vector4(0, -10, 0, 0);
                break;
            case "-":
                margin = new Vector4(0, -20, 0, -5);
                break;
            default:
                margin = textLable.margin;
                break;
        }
        textLable.margin = margin;
    }

    public virtual void SetRandom(int minValue, int maxValue)
    {
        int randomNumber = UnityEngine.Random.Range(minValue, maxValue);
        textLable.text = randomNumber.ToString();
        ValueUpdate();
    }

    public virtual void SetTextStyle(TMP_FontAsset font, Color color)
    {
        textLable.font = font;
        textLable.color = color;
    }

    public virtual void SetFontSize(float size)
    {
        textLable.fontSizeMax = size;
        textLable.fontSizeMin = size;
        textLable.fontSize = size;
    }

    #endregion

    #region GFX Methods

    protected void DoTextTween()
    {
        var sequence = DOTween.Sequence();

        if (isTextRotating)
        {
            sequence.Join(textTransform.DOShakeRotation(Random.Range(0.5f, 1f), textRotateTo, 40, 60).SetEase(Ease.InOutQuad));
        }
    }

    public virtual void SetImage(Sprite newImage)
    {
        image.sprite = newImage;
    }

    public virtual void SetAsGoal()
    {
        SetText("?");
        SetTextStyle(operatorFont, Color.white);
        DoTextTween();
    }

    #endregion
}
