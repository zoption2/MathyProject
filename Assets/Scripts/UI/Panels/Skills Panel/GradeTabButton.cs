using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.UI
{
    public enum GradeTabState
    {
        Default = 0,
        Selected = 1,
        Inactive = 2
    }

    public class GradeTabButton : MonoBehaviour
    {
        #region FIELDS

        [Header("COMPONENTS:")]
        [SerializeField] private Button button;
        [SerializeField] private Toggle toggle;
        [SerializeField] private Canvas canvasRenderer;
        [SerializeField] private Image inactiveBlocker;
        [SerializeField] private Image tabBaseImage;

        [Header("CONFIG:")]
        [SerializeField] private int defaultOrderInLayer = 1;
        [SerializeField] private int selectedOrderInLayer = 3;
        [SerializeField] private GradeTabState state = 0;
        public GradeTabState State
        {
            get => state;
            set
            {
                state = value;
                UpdateState();
            }
        }

        public Button Button { get => button; }

        #endregion

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            UpdateState();
        }

        private void UpdateState()
        {
            switch (state)
            {
                case GradeTabState.Default:                    
                    toggle.enabled = false;
                    button.interactable = true;
                    inactiveBlocker.enabled = false;
                    canvasRenderer.sortingOrder = defaultOrderInLayer;
                    break;
                case GradeTabState.Selected:
                    toggle.enabled = true;
                    button.interactable = false;
                    inactiveBlocker.enabled = false;
                    canvasRenderer.sortingOrder = selectedOrderInLayer;
                    break;
                case GradeTabState.Inactive:
                    toggle.enabled = false;
                    button.interactable = false;
                    inactiveBlocker.enabled = true;
                    canvasRenderer.sortingOrder = defaultOrderInLayer;
                    break;
                default:
                    goto case GradeTabState.Default;
            }
        }

        public void UpdateDisplayStyle(bool isMinimal)
        {
            if (isMinimal)
            {
                tabBaseImage.pixelsPerUnitMultiplier = 2.5f;
                inactiveBlocker.pixelsPerUnitMultiplier = 2.5f;
            }
            else
            {
                tabBaseImage.pixelsPerUnitMultiplier = 1.4f;
                inactiveBlocker.pixelsPerUnitMultiplier = 1.4f;
            }
        }
    }
}