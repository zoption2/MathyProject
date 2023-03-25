using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System;
using Mathy.Core.Tasks;

namespace Mathy.UI.Tasks
{
    public class ChallengeTaskBehaviour : TaskBehaviour
    {
        [SerializeField] 
        private Transform RightPanel;
        [SerializeField]
        private Transform LeftPanel;
        [SerializeField]
        private Transform TimerPanel;
        [SerializeField]
        private LivesPanel LivesPanel;
        [SerializeField]
        public Transform TaskPanel;
        [SerializeField]
        public Transform VariantsPanel;
        [SerializeField]
        private Image BGImage;

        private GameObject bgImageContainer;
        private FlexibleGridLayout vGrid;
        private RectTransform vPanel;
        private RectTransform rectImage;
        private RectTransform rectGrid;
        private RectTransform rectRight;
        private RectTransform rectLeft;
        private RectTransform rectTaskPanel;
        private AspectRatioFitter imageFitter;
        private AspectRatioFitter gridFitter;

        private int maxLives;
        private float panelsMarginX = 0;

        protected override async UniTask ClearAllPanelsAsync()
        {
            VariantsPanel.DestroyChildren();
            TaskPanel.DestroyChildren();
            await UniTask.Yield();
        }

        public override async System.Threading.Tasks.Task Initialize(Core.Tasks.Task task)
        {
            this.Task = task;
            if (VariantsPanel != null)
            {
                bgImageContainer = BGImage.transform.parent.gameObject;
                vGrid = VariantsPanel.GetComponent<FlexibleGridLayout>();
                vPanel = VariantsPanel.parent.GetComponent<RectTransform>();

                rectImage = bgImageContainer.GetComponent<RectTransform>();
                rectGrid = vGrid.GetComponent<RectTransform>();
                imageFitter = bgImageContainer.GetComponent<AspectRatioFitter>();
                gridFitter = vGrid.GetComponent<AspectRatioFitter>();
            }
            if (TaskPanel != null)
            {
                rectTaskPanel = TaskPanel.GetComponent<RectTransform>();
            }
            rectRight = RightPanel.GetComponent<RectTransform>();
            rectLeft = LeftPanel.GetComponent<RectTransform>();

            await ClearAllPanelsAsync();
        }

        public virtual float GetCorrectRate()
        {
            float correctRate = LivesPanel.Lives / (float)maxLives * 100f;
            return correctRate;
        }

        public async UniTask ResetToDefault()
        {
            gridFitter.enabled = true;
            imageFitter.enabled = true;
            await ClearAllPanelsAsync();
        }

        public void SetLives(int lives)
        {
            LivesPanel.SetLives(lives);
        }
        public void SetDamage(int damage)
        {
            LivesPanel.SetDamage(damage);
        }

        #region Display style

        protected virtual async UniTask UpdatePanels()
        {
            imageFitter.enabled = false;
            Vector2 center = new Vector2(0.5f, 0.5f);
            rectImage.anchorMin = center;
            rectImage.anchorMax = center;
            rectImage.pivot = center;

            await UniTask.WaitForFixedUpdate();

            gridFitter.aspectRatio = (float)vGrid.columns / (float)vGrid.rows;
            rectImage.sizeDelta = rectGrid.rect.size;
            UpdateTaskPanelSize();
            UpdatePanelPosition(rectLeft);
            UpdatePanelPosition(rectRight);
        }

        protected void UpdatePanelPosition(RectTransform panel)
        {
            bool isRight = panel.position.x > rectGrid.position.x;
            float panelWidth = panel.rect.size.x;
            float posX = (float)Math.Round((rectGrid.rect.size.x + panelWidth + panelsMarginX) / 2f, 0) * (isRight ? 1 : -1);

            panel.anchorMin = new Vector2(0.5f, panel.anchorMin.y);
            panel.anchorMax = new Vector2(0.5f, panel.anchorMax.y);
            panel.pivot = new Vector2(0.5f, 0.5f);

            panel.anchoredPosition = new Vector2(posX, panel.anchoredPosition.y);
            panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);
        }

        protected void UpdateTaskPanelSize()
        {
            float panelWidth = rectGrid.rect.size.x;
            rectTaskPanel.anchorMin = new Vector2(0.5f, rectTaskPanel.anchorMin.y);
            rectTaskPanel.anchorMax = new Vector2(0.5f, rectTaskPanel.anchorMax.y);
            rectTaskPanel.pivot = new Vector2(0.5f, 0.5f);
            rectTaskPanel.sizeDelta = new Vector2(panelWidth, rectTaskPanel.sizeDelta.y);
        }

        protected void ShowTaskPanel(bool isActive)
        {
            float anchorMaxY = isActive ? 0.8f : 1f;
            TaskPanel.gameObject.SetActive(isActive);
            vPanel.anchorMax = new Vector2(vPanel.anchorMax.x, anchorMaxY);
            rectLeft.anchorMax = new Vector2(rectLeft.anchorMax.x, anchorMaxY);
            rectRight.anchorMax = new Vector2(rectRight.anchorMax.x, anchorMaxY);
        }

        #region SHOW TIMER

        protected void ShowTimer(bool isActive, Transform container)
        {
            TimerPanel.gameObject.SetActive(isActive);
            TimerPanel.SetParent(container, false);
        }

        protected void ShowTimer(bool isActive)
        {
            TimerPanel.gameObject.SetActive(isActive);
        }

        #endregion

        #region SHOW LIVES PANEL

        protected void ShowLivesPanel(bool isActive, Transform container)
        {
            LivesPanel.gameObject.SetActive(isActive);
            LivesPanel.transform.SetParent(container, false);
        }

        protected void ShowLivesPanel(bool isActive)
        {
            LivesPanel.gameObject.SetActive(isActive);
        }

        #endregion

        #region SHOW BGIMAGE

        protected void ShowBGImage(bool isActive, Sprite image)
        {
            bgImageContainer.SetActive(isActive);
            BGImage.sprite = image;
        }

        protected void ShowBGImage(bool isActive)
        {
            bgImageContainer.SetActive(isActive);
        }

        #endregion

        #region SHOW VARIANTS PANEL

        protected async void ShowVariantsPanel(bool isActive, int top, Vector2 spacing)
        {
            vPanel.gameObject.SetActive(isActive);
            vGrid.padding.top = top;
            vGrid.spacing = spacing;
            if (isActive)
            {
                await UpdatePanels();
            }
        }

        #endregion

        #endregion

        #region IDisposable Support
        //! NOT IMPLEMENTED YET!!!!!!!!!!!!!!!!!!!!!!!!!
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    this.gameObject.SetActive(false);
                    ClearAllPanelsAsync();

                    Destroy(this.gameObject);
                }

                VariantsPanel = null;

                Task = null;

                disposedValue = true;
            }
        }

        public override void SetActiveViewPanels(bool isActive)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
