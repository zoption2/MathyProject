using UnityEngine.UI;
using UnityEngine;
using System;

namespace Mathy.UI
{
    public class TaskIndicatorButton : TaskIndicator
    {
        [SerializeField]
        public Button indicatorButton;

        public int Id { get; set; }

        public event EventHandler OnPressedEvent;

        private void Awake()
        {
            this.indicatorButton.onClick.AddListener(() => OnButtonPressedEvent(this, EventArgs.Empty));
        }

        private void OnButtonPressedEvent(object sender, EventArgs e)
        {
            this.OnPressedEvent.Invoke(sender, e);
        }

    }
}