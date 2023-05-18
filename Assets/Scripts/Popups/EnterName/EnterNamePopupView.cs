using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace Mathy.UI
{
    public interface IEnterNamePopupView : IView
    {
        event Action<string> ON_CLOSE_CLICK;
        event Action<string> ON_SAVE_CLICK;
        void Init(Camera camera, int orderLayer);
        void SetTitle(string title);
        void SetEnterNameText(string text);
        void SetDefaultPlayerName(string playerName);
        void SetSaveText(string text);
    }


    public class EnterNamePopupView : MonoBehaviour, IEnterNamePopupView
    {
        public event Action<string> ON_CLOSE_CLICK;
        public event Action<string> ON_SAVE_CLICK;

        private const int kMinNameCharacters = 1;
        private const int kMaxNameCharacters = 24;

        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _enterNameText;
        [SerializeField] private TMP_Text _defaultPlayerName;
        [SerializeField] private TMP_Text _saveText;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private BaseViewAnimator _animator;

        private string _defaultName;


        public void Init(Camera camera, int orderLayer)
        {
            _canvas.worldCamera = camera;
            _canvas.sortingOrder = orderLayer;
        }

        public void Show(Action onShow)
        {
            _animator.AnimateShowing(() =>
            {
                _closeButton.onClick.AddListener(DoOnCloseButtonClick);
                _saveButton.onClick.AddListener(DoOnSaveButtonClick);
                _inputField.onValueChanged.AddListener(DoOnNameChanged);
                onShow?.Invoke();
            });
        }

        public void Hide(Action onHide)
        {
            _closeButton.onClick.RemoveListener(DoOnCloseButtonClick);
            _saveButton.onClick.RemoveListener(DoOnSaveButtonClick);
            _inputField.onValueChanged.RemoveListener(DoOnNameChanged);
            _animator.AnimateHiding(onHide);
        }

        public void Release()
        {
            Destroy(gameObject);
        }

        public void SetDefaultPlayerName(string playerName)
        {
            _defaultPlayerName.text = playerName;
            _defaultName = playerName;
        }

        public void SetEnterNameText(string text)
        {
            _enterNameText.text = text;
        }

        public void SetSaveText(string text)
        {
            _saveText.text = text;
        }

        public void SetTitle(string title)
        {
            _title.text = title;
        }

        private void DoOnCloseButtonClick()
        {
            ON_CLOSE_CLICK?.Invoke(_defaultName);
        }

        private void DoOnNameChanged(string name)
        {
            bool isNameValid = ValidateName(name);
            _saveButton.interactable = isNameValid;
        }

        private void DoOnSaveButtonClick()
        {
            var name = _inputField.text;
            ON_SAVE_CLICK?.Invoke(name);
        }

        private bool ValidateName(string name)
        {
            // Check if the name is not empty
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            // Check if the name in correct characters range length
            if (name.Length < kMinNameCharacters && name.Length > kMaxNameCharacters)
            {
                return false;
            }

            // Check if the name starts with a space
            if (name.StartsWith(" "))
            {
                return false;
            }

            // Check if the name has double spaces
            if (name.Contains("  "))
            {
                return false;
            }

            return true;
        }
    }
}


