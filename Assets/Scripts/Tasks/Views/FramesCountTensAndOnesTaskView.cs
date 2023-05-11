﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Mathy.Core.Tasks.DailyTasks
{
    public interface IFramesCountTensAndOnesTaskView : ITaskView
    {
        ITaskElementHolderView[] ElementsHolder { get; }
        ITaskViewComponentClickable[] InputButtons { get; }
        ITaskViewComponent InputFieldElementTens { get; }
        ITaskViewComponent InputFieldElementOnes { get; }
        ITaskViewComponent ResultField { get; }
        void SetHeaderImage(Sprite sprite);
        void SetInputsHolderImage(Sprite sprite);
    }


    public sealed class FramesCountTensAndOnesTaskView : BaseTaskView, IFramesCountTensAndOnesTaskView
    {
        [SerializeField] private TaskElementView inputFieldTens;
        [SerializeField] private TaskElementView inputFieldOnes;
        [SerializeField] private TaskElementView resultField;
        [SerializeField] private TaskElementHolderView[] holders;
        [SerializeField] private Image[] inputsHolderImages;
        [SerializeField] private Image headerImage;
        [SerializeField] private TaskElementViewClickable[] inputButtons;

        public ITaskElementHolderView[] ElementsHolder => holders;
        public ITaskViewComponent InputFieldElementTens => inputFieldTens;
        public ITaskViewComponent InputFieldElementOnes => inputFieldOnes;
        public ITaskViewComponent ResultField => resultField;
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
    }
}

