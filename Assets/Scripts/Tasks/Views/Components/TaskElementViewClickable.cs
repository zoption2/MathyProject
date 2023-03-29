﻿using UnityEngine;
using UnityEngine.UI;
using Mathy.UI.Tasks;
using System;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskViewComponentClickable : ITaskViewComponent
    {
        event Action<ITaskViewComponent> ON_CLICK;
        bool IsInteractable { get; set; }
    }


    public class TaskElementViewClickable : TaskElementView, ITaskViewComponentClickable
    {
        public event Action<ITaskViewComponent> ON_CLICK;

        [SerializeField] private Button button;
        public bool IsInteractable { get => button.interactable; set => button.interactable = value; }

        public override void Init(int index, string value, TaskElementState initedState = TaskElementState.Default)
        {
            base.Init(index, value, initedState);
            button.onClick.AddListener(DoOnClick);
        }

        public override void Release()
        {
            button.onClick.RemoveListener(DoOnClick);
            base.Release();
        }

        private void DoOnClick()
        {
            ON_CLICK?.Invoke(this);
        }
    }
}

