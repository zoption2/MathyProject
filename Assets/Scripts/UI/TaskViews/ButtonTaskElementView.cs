using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Mathy.Core.Tasks;
using System;

namespace Mathy.UI.Tasks
{
    public class ButtonTaskElementView : TextTaskViewElement
    {
        [SerializeField]
        protected List<Sprite> statusBackground;
        [SerializeField]
        protected List<ParticleSystem> answerFx;
        [SerializeField]
        protected Button button;
        [SerializeField]
        protected GameObject buttonImage;

        public bool isPressed { get; private set; } = false;

        public event EventHandler OnButtonPressedEvent;

        public System.Threading.Tasks.Task SelectionTask { get; private set; }

        //Setting value to it
        protected virtual void SetValue(System.Object value)
        {
            System.Type valueType = value.GetType();
            if (valueType == typeof(int) || valueType == typeof(string))
            {
                this.textLable.text = value.ToString();
            }
            else if (valueType == typeof(ArithmeticSigns))
            {
                this.textLable.text = Convert.ToChar(value).ToString();
            }
            else
            {
                Debug.LogError("Unsupported type");
            }

        }

        public ButtonTaskElementView Initialization(Element element)
        {
            this.Element = element;
            SetValue(element.Value);

            button.onClick.AddListener(OnButtonPressed);

            return this;
        }

        private void OnButtonPressed()
        {
            AudioManager.Instance.ButtonClickSound();
            OnButtonPressedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void SetActiveVisual(bool isActive)
        {
            this.buttonImage.SetActive(isActive);
        }

        public void SetInteractable(bool isInteractable)
        {
            button.interactable = isInteractable;
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


        private void PlayFX(ParticleSystem fx)
        {
            fx.Stop();
            fx.Play();
        }

        public override void Dispose()
        {
            this.gameObject.SetActive(false);
            backgroundImage = null;
            textLable = null;
            statusBackground.Clear();
            answerFx.Clear();
            button.onClick.RemoveAllListeners();
            buttonImage = null;
            button = null;
            SelectionTask = null;

            Element = null;
            value = null;

            GameObject.Destroy(this.gameObject);
        }

    }
}