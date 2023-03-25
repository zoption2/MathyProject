﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mathy.UI;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskView : IView
    {
        event Action ON_EXIT_CLICK;
        public void SetTitle(string title);
        public void SetDescription(string description);

    }

    public interface IStandardTaskView : ITaskView
    {
        public Transform ElementsParent { get; }
        public Transform VariantsParent {get; }
    }

    public class StandardTaskView: MonoBehaviour, IStandardTaskView
    {
        public event Action ON_EXIT_CLICK;

        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Button exitButton;
        [SerializeField] private UIComponentFactory componentsFactory;

        [SerializeField] private Transform elementsPanel;
        [SerializeField] private Transform variantsPanel;
        [SerializeField] private GameObject progressParPanel;
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private StandardTaskViewAnimator animator;

        private List<ITaskViewComponent> components;

        public Transform ElementsParent => elementsPanel;
        public Transform VariantsParent => variantsPanel;

        public void SetTitle(string title)
        {
            titleText.text = title;
        }

        public void SetDescription(string description)
        {
            //not implemented
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

