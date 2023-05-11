using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Mathy.UI
{
    public interface IParentGatePopupView : IView
    {
        public event Action<string> ON_OK_CLICK;
        public event Action ON_CANCEL_CLICK;

        public void SetLabelText(string text);
        public void SetCapchaText(string text);
        public void SetOkText(string text);
        public void SetCancelText(string text);
        public void ResetInputField();
    }

    public class ParentGatePopupView : MonoBehaviour, IParentGatePopupView
    {
        public event Action<string> ON_OK_CLICK;
        public event Action ON_CANCEL_CLICK;

        public const int kMaxInputedChars = 2;

        [SerializeField] private TMP_Text labelText;
        [SerializeField] private TMP_Text capchaText;
        [SerializeField] private TMP_Text cancelText;
        [SerializeField] private TMP_Text okText;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button okButton;
        [SerializeField] private BaseViewAnimator animator;

        private string inputedValue = "";


        public void Show(Action onShow)
        {
            animator.AnimateShowing(() =>
            {
                SubscribeInputs();
                InitInputField();
                onShow?.Invoke();
            });
        }

        public void Hide(Action onHide)
        {
            UnsubscribeInputs();
            animator.AnimateHiding(() =>
            {
                onHide?.Invoke();
            });
        }

        public void SetLabelText(string text)
        {
            labelText.text = text;
        }

        public void SetCapchaText(string text)
        {
            capchaText.text = text;
        }

        public void SetOkText(string text)
        {
            okText.text = text;
        }

        public void SetCancelText(string text)
        {
            cancelText.text = text;
        }

        public void ResetInputField()
        {
            inputField.text = string.Empty;
            inputedValue = string.Empty;
        }

        public void Release()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

#region PrivatMethods

        private void OnInputFieldChanged(string value)
        {
            inputedValue = value;
        }

        private void OnOkButtonClick()
        {
            ON_OK_CLICK?.Invoke(inputedValue);
        }

        private void OnCancelButtonClick()
        {
            ON_CANCEL_CLICK?.Invoke();
        }

        private void SubscribeInputs()
        {
            cancelButton.onClick.AddListener(OnCancelButtonClick);
            okButton.onClick.AddListener(OnOkButtonClick);
            inputField.onEndEdit.AddListener(OnInputFieldChanged);
        }

        private void UnsubscribeInputs()
        {
            cancelButton.onClick.RemoveListener(OnCancelButtonClick);
            okButton.onClick.RemoveListener(OnOkButtonClick);
            inputField.onEndEdit.RemoveListener(OnInputFieldChanged);
        }

        private void InitInputField()
        {
            inputField.characterLimit = kMaxInputedChars;
            inputField.ActivateInputField();
        }

        #endregion
    }
}

