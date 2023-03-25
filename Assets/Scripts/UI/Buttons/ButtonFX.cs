using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using Mathy.Core;

public class ButtonFX : MonoBehaviour
{
    #region FIELDS

    protected bool isPressed = false;
    protected RectTransform rTransform;
    protected Button button;
    
    [Header("Tween:")]
    
    [SerializeField] protected bool isRotating = false;
    [SerializeField] protected bool isScaling = true;
    [SerializeField] protected float tweenDuration = 0.5f;
    [SerializeField] protected Transform rotatingElement;
    [SerializeField] protected Vector3 scaleTo = new Vector3(0.05f, 0.1f, 0);
    [SerializeField] protected Vector3 rotateTo = new Vector3(0, 0, 5);
    
    public UnityEvent OnTweenCompleteEvent;

    [Header("Config:")]

    [SerializeField] private bool isSound = true;

    #endregion

    protected virtual void Awake()
    {
        Initialization();
    }

    /*protected virtual void Start()
    {
        if (isSound) SetSound();
    }*/

    protected virtual void Initialization()
    {
        if (rotatingElement == null) rotatingElement = transform;
        rTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnPress);
        //if (isSound) SetSound();
    }

    protected virtual void SetSound()
    {
        button.onClick.AddListener(AudioManager.Instance.ButtonClickSound);
    }

    public virtual void OnPress()
    {
        if (!isPressed)
        {
            isPressed = true;            
            DoTween();
            Vibrate();
        }
        if (isSound)
        {
            AudioManager.Instance.ButtonClickSound();
        }
    }

    protected virtual void Vibrate()
    {
        VibrationManager.Instance.TapVibrateCustom();
    }

    protected virtual void DoTween()
    {
        var sequence = DOTween.Sequence();

        if (isScaling)
        {
            sequence.Join(rTransform.DOShakeScale(tweenDuration, scaleTo, 10, 60));
        }
        if (isRotating)
        {
            sequence.Join(rotatingElement.DOShakeRotation(tweenDuration, rotateTo, 40, 60).SetEase(Ease.InOutQuad));
        }
        sequence.OnComplete(() => OnTweenComplete());
    }

    protected virtual void OnTweenComplete()
    {
        isPressed = false;
        OnTweenCompleteEvent.Invoke();
    }
}
