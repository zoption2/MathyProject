using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Mathy.Core.Tasks;
using System;

namespace Mathy.UI.Tasks
{
    public class VariantView : TextTaskViewElement
    {
        [SerializeField] 
        protected List<Sprite> statusBackground;
        //[SerializeField] 
        //protected List<ParticleSystem> answerFx;
        [SerializeField]
        protected Button button;
        //[SerializeField]
        //protected GameObject buttonImage;


        [Header("Tween settings:")]
        [SerializeField] protected bool isRotating = false;
        [SerializeField] protected bool isScaling = true;
        [SerializeField] protected Vector3 scaleTo = new Vector3(0.1f, 0.2f, 0);
        [SerializeField] protected Vector3 rotateTo = new Vector3(0, 0, 10);
        [SerializeField] protected float tweenDuration = 1.0f;
        public Variant Variant { get; private set; }

        public override object Value
        {
            get => value;
            set
            {
                this.value = value;
                //If value type is char, string or int
                if (value.GetType() == typeof(string) || value.GetType() == typeof(int) || value.GetType() == typeof(char))
                {
                    textLable.text = value.ToString();
                }
            }
        }

        public bool isPressed { get; private set; } = false;

        public event EventHandler OnButtonPressedEvent;
        public System.Threading.Tasks.Task TweenTask { get; private set; }
        public System.Threading.Tasks.Task SelectionTask { get; private set; }

        public virtual void SetAsCorrect()
        {
            SetBackgroundImage(statusBackground[1]);
        }
        public virtual void SetAsWrong()
        {
            SetBackgroundImage(statusBackground[2]);
        }
        public virtual void SetAsSelected()
        {
            SetBackgroundImage(statusBackground[4]);
        }

        public virtual void SetAsCorrect(float duration)
        {
            if (!isPressed)
            {
                AudioManager.Instance.CorrectVariantSound();

                isPressed = true;
                SetInteractable(false);

                SetBackgroundImage(statusBackground[1]);
                DoTween(false, false, duration);
                //PlayFX(answerFx[0]);
            }
        }

        public virtual void SetAsWrong(float duration)
        {
            if (!isPressed)
            {
                AudioManager.Instance.WrongVariantSound();

                isPressed = true;
                SetInteractable(false);

                SetBackgroundImage(statusBackground[2]);
                DoTween(false, false, duration);
                //PlayFX(answerFx[1]);
            }
        }

        //Setting value to it
        protected virtual void SetValue(System.Object value)
        {
            System.Type valueType = value.GetType();
            if (valueType == typeof(int) || valueType == typeof(string) || valueType == typeof(bool))
            {
                this.textLable.text = value.ToString();
            }
            else if(valueType == typeof(ArithmeticSigns))
            {
                this.textLable.text = Convert.ToChar(value).ToString();
            }
            else
            {
                Debug.LogError("Unsupported type");
            }
            SetTextOffsets();
        }

        public VariantView Initialization(Variant variant)
        {
            this.Variant = variant;
            SetValue(variant.Value);

            button.onClick.AddListener(OnButtonPressed);

            return this;
        }

        private void OnButtonPressed()
        {
            AudioManager.Instance.ButtonClickSound();
            OnButtonPressedEvent?.Invoke(this, EventArgs.Empty);

            if (this.Variant.IsVariantCorrect)
            {
                SetAsCorrect(tweenDuration);
            }
            else
            {
                SetAsWrong(tweenDuration);
            }
        }

        public void SetInteractable(bool isInteractable)
        {
            button.interactable = isInteractable;
        }

        #region Tween Animation
        protected virtual void DoTween(bool isCorrect, bool isHide, float tweenDuration)
        {
            Sequence sequence = DOTween.Sequence();
            TweenTask = sequence.AsyncWaitForCompletion();
            if (isScaling)
            {
                sequence.Join(backgroundImage.transform.DOShakeScale(tweenDuration, scaleTo, 10, 60f));
            }
            if (isRotating)
            {
                sequence.Join(backgroundImage.transform.DOShakeRotation(tweenDuration, rotateTo, 40, 60).SetEase(Ease.InOutQuad));
            }
            if (isHide)
            {
                sequence.Append(textLable.transform.DOScale(0, tweenDuration));
                sequence.Append(backgroundImage.DOFade(0, tweenDuration));
            }
            //sequence.OnComplete(() => OnTweenComplete(isCorrect));
        }

        public void SelectTween(bool showValue)
        {
            isPressed = true;
            Sequence selection = DOTween.Sequence();
            SelectionTask = selection.AsyncWaitForCompletion();
            selection.Join(backgroundImage.transform.DORotate(new Vector3(0, 90, 0), 0.25f).SetEase(Ease.InOutQuad));
            selection.Append(textLable.DOFade(showValue ? 1 : 0, 0.1f));
            selection.Join(backgroundImage.DOCrossfadeImage(showValue ? statusBackground[0] : statusBackground[3], 0.1f));
            selection.Append(backgroundImage.transform.DORotate(new Vector3(0, 0, 0), 0.25f).SetEase(Ease.InOutQuad));
            selection.OnComplete(() => isPressed = false);
        }

        public void SelectTween2(bool isSelected)
        {
            Sequence selection = DOTween.Sequence();
            SelectionTask = selection.AsyncWaitForCompletion();
            selection.Join(backgroundImage.transform.DOScale(isSelected ? new Vector2(1.1f, 1.1f) : new Vector2(1f, 1f), 0.25f).SetEase(Ease.InOutQuad));
        }

        private void PlayFX(ParticleSystem fx)
        {
            fx.Stop();
            fx.Play();
        }

        protected void SetTextOffsets()
        {
            int bottom = textLable.text == ">" || textLable.text == "<" ? 64 : 32;
            textLable.rectTransform.SetBottom(bottom);
        }

        #endregion

    }
}