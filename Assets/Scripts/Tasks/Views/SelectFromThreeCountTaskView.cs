using Mathy.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ISelectFromThreeCountTaskView : ITaskView
    {
        event Action ON_HELP_CLICK;
        Transform[] GroupParents { get; }
        void SetHeaderImage(Sprite sprite);
        Vector2 GetRandomPositionAtGroup(int groupIndex);
        ITaskViewComponentClickable[] Inputs { get; }
    }


    public class SelectFromThreeCountTaskView : MonoBehaviour, ISelectFromThreeCountTaskView
    {
        public event Action ON_HELP_CLICK;
        public event Action ON_EXIT_CLICK;

        private const int kFirstGroupIndex = 0;
        private const int kSecondGroupIndex = 1;
        private const int kThirdGroupIndex = 2;

        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button helpButton;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image headerImage;
        [SerializeField] private RectTransform[] groupParents;
        [SerializeField] private BaseViewAnimator animator;
        [SerializeField] private TaskButtonVariantClickable[] inputs;

        public ITaskViewComponentClickable[] Inputs => inputs;
        public Transform[] GroupParents => groupParents;


        public void SetHeaderImage(Sprite sprite)
        {
            headerImage.sprite = sprite;
        }

        public Vector2 GetRandomPositionAtGroup(int groupIndex)
        {
            var size = groupParents[groupIndex].rect.size;
            float xPos = UnityEngine.Random.Range(-(size.x / 2) + 50, size.x / 2 - 50);
            float yPos = UnityEngine.Random.Range(-(size.y / 2) + 50, size.y / 2 - 50);
            size.x = xPos;
            size.y = yPos;
            Debug.LogFormat("Position of Image selected as {0}", size);
            return size;
        }

        public void SetTitle(string title)
        {
            titleText.text = title;
        }

        public void SetDescription(string description)
        {
            throw new NotImplementedException();
        }

        public void SetBackground(Sprite image)
        {
            backgroundImage.sprite = image;
        }

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

        private void DoOnExitButtonClick()
        {
            ON_EXIT_CLICK?.Invoke();
        }
    }
}

