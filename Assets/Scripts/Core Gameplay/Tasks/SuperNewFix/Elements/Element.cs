using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    public abstract class Element : IDisposable, IAsyncDisposable
    {
        public virtual object Value { get; protected set; }

        public TaskViewElement ElementView { get; protected set; }

        protected Transform viewParent;

        public abstract UniTask CreateView(Transform parent, TaskType taskType);

        protected abstract UniTask LoadAsset(string name);


        #region IDisposable Support
        protected bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ElementView?.Dispose();
                }

                Value = null;
                ElementView = null;
                viewParent = null;

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        public virtual async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            Dispose(false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (ElementView is IAsyncDisposable disposable)
            {
                await disposable.DisposeAsync().ConfigureAwait(false);
            }
            else
            {
                ElementView?.Dispose();
            }
        }

        #endregion
    }

}