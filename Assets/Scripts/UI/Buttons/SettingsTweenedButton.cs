using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mathy.UI
{
    public class SettingsTweenedButton : TweenedButton
    {
        [SerializeField] protected CanvasGroup canvasGroup;
        public override bool IsInteractable
        {
            get => button.interactable;
            set
            {
                button.interactable = value;
                UpdateInteractable();
            }
        }

        private void UpdateInteractable()
        {
            canvasGroup.alpha = button.interactable ? 1f : 0.5f;
        }

        protected override void Initialize()
        {
            base.Initialize();
            UpdateInteractable();
        }
    }
}
