using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface IFramesCountToTenTaskView : ITaskView
    {
        event Action ON_HELP_CLICK;
        void SetHeaderImage(Sprite sprite);
        void SetInputsHolderImage(Sprite sprite);
        ITaskElementFrame[] Frames { get; }
        ITaskViewComponentClickable[] Inputs { get; }
    }

    public class FramesCountToTenTaskView : BaseTaskView, IFramesCountToTenTaskView
    {
        [SerializeField] private Image inputsHolderImage;
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

