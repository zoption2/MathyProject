using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface IFramesCountToTenTaskView : ITaskView
    {
        void SetHeaderImage(Sprite sprite);
        void SetInputsHolderImage(Sprite sprite);
        ITaskElementFrame[] Frames { get; }
        ITaskViewComponentClickable[] Inputs { get; }
    }

    public sealed class FramesCountToTenTaskView : BaseTaskView, IFramesCountToTenTaskView
    {
        [SerializeField] private Image inputsHolderImage;
        [SerializeField] private Image headerImage;
        [SerializeField] private TaskElementFrame[] frames;
        [SerializeField] private TaskElementViewClickable[] inputs;


        public ITaskElementFrame[] Frames => frames;
        public ITaskViewComponentClickable[] Inputs => inputs;


        public void SetHeaderImage(Sprite sprite)
        {
            headerImage.sprite = sprite;
        }

        public void SetInputsHolderImage(Sprite sprite)
        {
            inputsHolderImage.sprite = sprite;
        }
    }
}

