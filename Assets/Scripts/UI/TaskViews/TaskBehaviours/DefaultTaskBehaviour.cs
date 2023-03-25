using System;
using UnityEngine;
using Mathy.Core.Tasks;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System;

namespace Mathy.UI.Tasks
{
    public class DefaultTaskBehaviour : TaskBehaviour
    {
        [Header("GUI Panels:")]
        [SerializeField]
        public Transform ElementsPanel;
        [SerializeField]
        public Transform VariantsPanel;
        [SerializeField]
        public ProgressBar ProgressBar;

        public async override System.Threading.Tasks.Task Initialize(Core.Tasks.Task task)
        {
            //Progress bar disabled by default
            SetActiveProgressBar(false);
            await base.Initialize(task);
            //View disabled by default
            SetActiveViewPanels(false);
        }

        public override void SetActiveViewPanels(bool isActive)
        {
            ElementsPanel.gameObject.SetActive(isActive);
            VariantsPanel.gameObject.SetActive(isActive);
        }

        public void StartTimer(float time)
        {
            ProgressBar.StartTimer(time);
        }
        public void StopTimer()
        {
            ProgressBar.StopTimer();
        }
        public void SetActiveProgressBar(bool isActive)
        {
            ProgressBar.gameObject.SetActive(isActive);
        }
        public void RestartProgressBarTimer()
        {
            ProgressBar.RestartTimer();
        }

        protected override async UniTask ClearAllPanelsAsync()
        {
            ClearElements();
            ClearVariants();
            await UniTask.Yield();
        }

        private void ClearElements()
        {
            ElementsPanel.DestroyChildren();
        }

        private void ClearVariants()
        {
            VariantsPanel.DestroyChildren();
        }

        #region IDisposable Support
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

                ElementsPanel = null;
                VariantsPanel = null;
                ProgressBar = null;

                Task = null;

                disposedValue = true;
            }
        }
        #endregion
    }
}
