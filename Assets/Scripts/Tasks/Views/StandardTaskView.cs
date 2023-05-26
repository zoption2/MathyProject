using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mathy.UI;
using System;
using System.Collections.Generic;


namespace Mathy.Core.Tasks.DailyTasks
{

    public interface IStandardTaskView : ITaskView
    {
        public Transform ElementsParent { get; }
        public Transform VariantsParent {get; }
        void SetHeaderImage(Sprite sprite);
        void SetInputsHolderImage(Sprite sprite);
    }

    public class StandardTaskView: BaseTaskView, IStandardTaskView
    {
        [SerializeField] private Image headerImage;
        [SerializeField] private Image inputsHolderImage;

        [SerializeField] private Transform elementsPanel;
        [SerializeField] private Transform variantsPanel;

        public Transform ElementsParent => elementsPanel;
        public Transform VariantsParent => variantsPanel;


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

