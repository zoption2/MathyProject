using Mathy.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskView : IView
    {
        event Action ON_EXIT_CLICK;
        event Action ON_HELP_CLICK;
        public void SetTitle(string title);
        public void SetDescription(string description);
        public void SetBackground(Sprite image);
    }


    public class BaseTaskView : MonoBehaviour, ITaskView
    {
        public event Action ON_HELP_CLICK;
        public event Action ON_EXIT_CLICK;

        [SerializeField] protected TMP_Text titleText;
        [SerializeField] protected Button exitButton;
        [SerializeField] protected Button helpButton;
        [SerializeField] protected Image backgroundImage;
        [SerializeField] protected BaseViewAnimator animator;

        public virtual void Show(Action onShow)
        {
            gameObject.SetActive(true);
            exitButton.onClick.AddListener(DoOnExitButtonClick);
            helpButton.onClick.AddListener(DoOnHelpButtonClick);
            onShow?.Invoke();
        }

        public virtual void Hide(Action onHide)
        {
            exitButton.onClick.RemoveListener(DoOnExitButtonClick);
            helpButton.onClick.RemoveListener(DoOnHelpButtonClick);
            animator.AnimateHiding(() =>
            {
                gameObject.SetActive(false);
                onHide?.Invoke();
            });
        }

        public virtual void Release()
        {
            Destroy(gameObject);
        }

        public void SetBackground(Sprite image)
        {
            backgroundImage.sprite = image;
        }

        public void SetDescription(string description)
        {
            throw new NotImplementedException();
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

        private void DoOnHelpButtonClick()
        {
            ON_HELP_CLICK?.Invoke();
        }
    }
}

