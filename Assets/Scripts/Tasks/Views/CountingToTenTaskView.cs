using Mathy.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ICountingToTenTaskView : ITaskView
    {
        event Action ON_HELP_CLICK;
        event Action<string> ON_VARIANT_CLICK;
        Transform ElementsHolder { get; }
        void SetHeaderImage(Sprite sprite);
        void SetInputsHolderImage(Sprite sprite);
    }

    public class CountingToTenTaskView : MonoBehaviour, ICountingToTenTaskView
    {
        public event Action ON_HELP_CLICK;
        public event Action<string> ON_VARIANT_CLICK;
        public event Action ON_EXIT_CLICK;

        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button helpButton;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image headerImage;
        [SerializeField] private Image inputsHolderImage;
        [SerializeField] private Transform elementsHolder;
        [SerializeField] private BaseViewAnimator animator;
        [SerializeField] private TaskNumerableButton[] inputs;

        public Transform ElementsHolder => elementsHolder;

        public void Show(Action onShow)
        {
            gameObject.SetActive(true);
            exitButton.onClick.AddListener(DoOnExitButtonClick);
            SubscribeToInputs();
            onShow?.Invoke();
        }

        public void Hide(Action onHide)
        {
            exitButton.onClick.RemoveListener(DoOnExitButtonClick);
            UnsubscribeFromInputs();
            animator.AnimateHiding(() =>
            {
                gameObject.SetActive(false);
                onHide?.Invoke();
            });
        }

        public void Release()
        {
            Destroy(gameObject);
        }

        public void SetBackground(Sprite image)
        {
            backgroundImage.sprite = image;
        }

        public void SetDescription(string description)
        {
            
        }

        public void SetHeaderImage(Sprite sprite)
        {
            headerImage.sprite = sprite;
        }

        public void SetInputsHolderImage(Sprite sprite)
        {
            inputsHolderImage.sprite = sprite;
        }

        public void SetTitle(string title)
        {
            titleText.text = title;
        }

        private void DoOnExitButtonClick()
        {
            VibrationManager.Instance.TapVibrateCustom();
            AudioManager.Instance.ButtonClickSound();
            ON_EXIT_CLICK?.Invoke();
        }

        private void SubscribeToInputs()
        {
            for (int i = 0, j = inputs.Length; i < j; i++)
            {
                inputs[i].ON_CLICK += DoOnInputClick;
            }
        }

        private void UnsubscribeFromInputs()
        {
            for (int i = 0, j = inputs.Length; i < j; i++)
            {
                inputs[i].ON_CLICK -= DoOnInputClick;
            }
        }

        private void DoOnInputClick(string value)
        {
            ON_VARIANT_CLICK?.Invoke(value);
        }
    }
}

