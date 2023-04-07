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
        Transform ElementsHolder { get; }
        void SetHeaderImage(Sprite sprite);
        void SetInputsHolderImage(Sprite sprite);
        Vector2 GetRandomPositionAtHolder();
        ITaskViewComponentClickable[] Inputs { get; }
    }

    public class CountingToTenTaskView : MonoBehaviour, ICountingToTenTaskView
    {
        public event Action ON_HELP_CLICK;
        public event Action ON_EXIT_CLICK;

        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button helpButton;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image headerImage;
        [SerializeField] private Image inputsHolderImage;
        [SerializeField] private RectTransform elementsHolder;
        [SerializeField] private BaseViewAnimator animator;
        [SerializeField] private TaskElementViewClickable[] inputs;

        public ITaskViewComponentClickable[] Inputs => inputs;
        public Transform ElementsHolder => elementsHolder;

        public void Show(Action onShow)
        {
            gameObject.SetActive(true);
            exitButton.onClick.AddListener(DoOnExitButtonClick);
            onShow?.Invoke();
        }

        public void Hide(Action onHide)
        {
            exitButton.onClick.RemoveListener(DoOnExitButtonClick);
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

        public Vector2 GetRandomPositionAtHolder()
        {
            var size = elementsHolder.rect.size;
            float xPos = UnityEngine.Random.Range(-(size.x/2) + 50, size.x/2 - 50);
            float yPos = UnityEngine.Random.Range(-(size.y/2) + 50, size.y/2 - 50);
            size.x = xPos;
            size.y = yPos;
            return size;
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
            ON_EXIT_CLICK?.Invoke();
        }
    }
}

