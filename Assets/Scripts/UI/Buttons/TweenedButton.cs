using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using Mathy.Core;

namespace Mathy.UI
{
    public class TweenedButton : MonoBehaviour
    {
        #region FIELDS

        [Header("BASIC CONFIG:")]
        [SerializeField] protected Button button;
        [SerializeField] protected RectTransform tweenTransform;
        [SerializeField] protected float tweenDuration = 0.5f;
        [SerializeField] protected Vector2 scaleTo = new Vector2(0.1f, 0.1f);
        [SerializeField] protected bool isSoundOnPress = true;
        [SerializeField] protected bool isVibroOnPress = true;

        [Header("GUI COMPONENTS:")]
        [SerializeField] protected Image iconImage;
        [SerializeField] protected Image buttonImage;

        public Button Button{ get => button; }
        public UnityEvent OnClick { get => button.onClick; }

        public virtual bool IsInteractable
        {
            get => button.interactable;
            set
            {
                button.interactable = value;
                iconImage.SetAlpha(value ? 1f : 0.5f);
            }
        }

        protected RectTransform rTransform;
        bool isPressed = false;

        #endregion

        #region INITIALIZATION

        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            if (button is null)
            {
                if (!TryGetComponent(out button)) gameObject.AddComponent<Button>();
            }
            rTransform = GetComponent<RectTransform>();
            if (tweenTransform is null) tweenTransform = rTransform;
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnPress);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnPress);
        }

        #endregion

        #region ON PRESS

        protected virtual void OnPress()
        {
            if (!isPressed)
            {
                isPressed = true;
                PlayOnPressTween();
                PlayOnPressSound();
                Vibrate();
            }
        }

        public void ResetOnPressTween()
        {
            button.onClick.AddListener(OnPress);
        }

        protected virtual void PlayOnPressTween()
        {
            tweenTransform.DOPunchScale(scaleTo, tweenDuration,5).
                OnComplete(() => OnTweenComplete());
        }

        protected virtual void PlayOnPressSound()
        {
            if (isSoundOnPress) AudioManager.Instance.ButtonClickSound();
        }

        protected virtual void Vibrate()
        {
            if (isVibroOnPress) VibrationManager.Instance.TapVibrateCustom();
        }

        protected virtual void OnTweenComplete()
        {
            isPressed = false;
        }

        #endregion
    }
}