using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AnswerVariantOLD : TaskElementOLD
{
    [SerializeField] List<Sprite> answerStatusBG;
    [SerializeField] protected List<ParticleSystem> answetFx;

    [Header("Tween:")]
    [SerializeField] protected bool isRotating = false;
    [SerializeField] protected bool isScaling = true;
    [SerializeField] protected float tweenDuration = 1.5f;
    [SerializeField] protected Vector3 scaleTo = new Vector3(0.1f, 0.2f, 0);
    [SerializeField] protected Vector3 rotateTo = new Vector3(0, 0, 10);

    public virtual string answerValue { get; set; }
    public bool isPressed { get; private set; } = false;
    public System.Threading.Tasks.Task tweenTask { get; private set; }
    public System.Threading.Tasks.Task selectionTask { get; private set; }
    protected Button button;

    void Start()
    {
        Initialization();
    }

    // Temp solution, need to fix this class later
    public void SetAsCorrect()
    {
        SetImage(answerStatusBG[1]);
    }

    public virtual void SetAsCorrect(float duration, bool isHide)
    {
        if (!isPressed)
        {
            isPressed = true;
            tweenDuration = duration;
            button.interactable = false;
            SetImage(answerStatusBG[1]);
            DoTween(true, isHide);
            //PlayFX(answetFx[0]);
        }
    }

    // Temp solution, need to fix this class later
    public void SetAsWrong()
    {
        SetImage(answerStatusBG[2]);
    }

    public virtual void SetAsWrong(float duration)
    {
        if (!isPressed)
        {
            isPressed = true;
            tweenDuration = duration;
            button.interactable = false; 
            SetImage(answerStatusBG[2]);
            DoTween(false, false);
            //PlayFX(answetFx[1]);
        }
    }

    protected virtual void Initialization()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { AudioManager.Instance.ButtonClickSound(); });
    }

    public void SetInteractable(bool isInteractable)
    {
        button = GetComponent<Button>();
        button.interactable = isInteractable;
    }

    protected virtual void DoTween(bool isCorrect, bool isHide)
    {
        Sequence sequence = DOTween.Sequence();
        tweenTask = sequence.AsyncWaitForCompletion();
        if (isScaling)
        {
            sequence.Join(image.transform.DOShakeScale(tweenDuration, scaleTo, 10, 60f));
        }
        if (isRotating)
        {
            sequence.Join(image.transform.DOShakeRotation(tweenDuration, rotateTo, 40, 60).SetEase(Ease.InOutQuad));
        }
        if (isHide)
        {
            sequence.Append(textLable.transform.DOScale(0, tweenDuration / 2));
            sequence.Append(image.DOFade(0, tweenDuration / 2));
        }
        sequence.OnComplete(() => OnTweenComplete(isCorrect));
    }

    public void SelectTween(bool showValue)
    {
        isPressed = true;
        Sequence selection = DOTween.Sequence();
        selectionTask = selection.AsyncWaitForCompletion();
        selection.Join(image.transform.DORotate(new Vector3(0, 90, 0), 0.25f).SetEase(Ease.InOutQuad));
        selection.Append(textLable.DOFade(showValue ? 1 : 0, 0.1f));
        selection.Join(image.DOCrossfadeImage(showValue ? answerStatusBG[0] : answerStatusBG[3], 0.1f));
        selection.Append(image.transform.DORotate(new Vector3(0, 0, 0), 0.25f).SetEase(Ease.InOutQuad));
        selection.OnComplete(() => isPressed = false);
    }

    public void SelectTween2(bool isSelected)
    {
        Sequence selection = DOTween.Sequence();
        selectionTask = selection.AsyncWaitForCompletion();
        selection.Join(image.transform.DOScale(isSelected ? new Vector2(1.1f, 1.1f) : new Vector2(1f, 1f), 0.25f).SetEase(Ease.InOutQuad));
    }

    public void SelectToTask(TaskElementOLD target)
    {
        float targetHeight = target.imageSize().y;
        Vector2 scaleTo = new Vector2(1.1f, 1.1f);
        Vector2 moveTo = target.transform.position;
        Vector2 sizeTo = new Vector2(targetHeight, targetHeight);
        Vector2 anchor = new Vector2(0.5f, 0.5f);
        SetImageAnchor(anchor);
        SelectToTaskTween(scaleTo, moveTo, sizeTo, 0.25f);
    }

    public void SelectToTask(bool isSelected)
    {
        Vector2 scaleTo = Vector2.one;
        Vector2 moveTo = transform.position;
        Vector2 sizeTo = Vector2.zero;
        if (isSelected)
        {
            scaleTo = new Vector2(1.1f, 1.1f);
            moveTo = transform.position;
        }
        SetImageAnchor();
        SelectToTaskTween(scaleTo, moveTo, sizeTo, 0.25f);
    }

    private void SelectToTaskTween(Vector2 scaleTo, Vector2 moveTo, Vector2 sizeTo, float duration)
    {
        Sequence selection = DOTween.Sequence();
        selectionTask = selection.AsyncWaitForCompletion();

        selection.Join(image.transform.DOScale(scaleTo, duration / 2).SetEase(Ease.InOutQuad));
        selection.Append(image.transform.DOScale(Vector2.one, duration / 2).SetEase(Ease.InOutQuad));
        selection.Join(image.transform.DOMove(moveTo, duration).SetEase(Ease.InOutQuad));
        selection.Join(rectImage.DOSizeDelta(sizeTo, duration).SetEase(Ease.InOutQuad));
    }

    private void PlayFX(ParticleSystem fx)
    {
        fx.Stop();
        fx.Play();
    }

    protected virtual void OnTweenComplete(bool isCorrect)
    {
        isPressed = false;
        button.interactable = true;
        if (isCorrect)
        {
            image.gameObject.SetActive(false);
        }
        else
        {
            SetImage(answerStatusBG[0]);
        }
    }
}
