using System;
using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Mathy.UI.Tasks
{
    public abstract class TaskBehaviour : MonoBehaviour, IDisposable, IAsyncDisposable
    {
        public Mathy.Core.Tasks.Task Task { get; protected set; }

        public virtual async System.Threading.Tasks.Task Initialize(Mathy.Core.Tasks.Task task)
        {
            this.Task = task;
            await ClearAllPanelsAsync();
        }

        protected abstract UniTask ClearAllPanelsAsync();

        #region IDisposable Support
        protected bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
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

                Task = null;

                disposedValue = true;
            }
        }

        public abstract void SetActiveViewPanels(bool isActive);

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            Dispose(false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            //GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }


        protected virtual async ValueTask DisposeAsyncCore()
        {
            SetActiveViewPanels(false);
            this.gameObject.SetActive(false);
            await ClearAllPanelsAsync();

            Destroy(this.gameObject);
        }

        #endregion
    }
}