using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Mathy.Core.Tasks.DailyTasks
{
    public interface IFramesCountToTwentyTaskView : ITaskView
    {
        Transform[] ElementsHolder { get; }
        ITaskViewComponentClickable[] InputButtons { get; }
        ITaskViewComponentClickable InputFieldElement { get; }
        void SetHeaderImage(Sprite sprite);
        void SetInputsHolderImage(Sprite sprite);
        void SetThereAreText(string text);
        void SetObjectNameText(string text);
    }


    public sealed class FramesCountToTwentyTaskView : BaseTaskView, IFramesCountToTwentyTaskView
    {
        [SerializeField] private TaskElementViewClickable inputField;
        [SerializeField] private Transform[] holders;
        [SerializeField] private Image[] inputsHolderImages;
        [SerializeField] private Image headerImage;
        [SerializeField] private TMP_Text thereAreText;
        [SerializeField] private TMP_Text objectNameText;
        [SerializeField] private TaskElementViewClickable[] inputButtons;

        public Transform[] ElementsHolder => holders;
        public ITaskViewComponentClickable InputFieldElement => inputField;
        public ITaskViewComponentClickable[] InputButtons => inputButtons;  

        public void SetHeaderImage(Sprite sprite)
        {
            headerImage.sprite = sprite;
        }

        public void SetInputsHolderImage(Sprite sprite)
        {
            for (int i = 0, j = inputsHolderImages.Length; i < j; i++)
            {
                inputsHolderImages[i].sprite = sprite;
            }
        }

        public void SetObjectNameText(string text)
        {
            objectNameText.text = text;
        }

        public void SetThereAreText(string text)
        {
            thereAreText.text = text;
        }
    }
}

