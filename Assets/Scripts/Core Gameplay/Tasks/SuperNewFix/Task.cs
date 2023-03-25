using CustomRandom;
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

namespace Mathy.Core.Tasks
{
    //Technically I have wrong IDisposable realization, need to be fixed
    //https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync
    // https://ru.stackoverflow.com/questions/681382/%D0%98%D1%81%D0%BF%D0%BE%D0%BB%D1%8C%D0%B7%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5-configureawaitfalse
    // https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync
    public abstract class Task : IAsyncDisposable, IDisposable
    {
        public List<Element> Elements = new List<Element>();

        protected FastRandom Random { get; set; }

        public int Seed { get; protected set; }
        public int ElementsAmount
        {
            get => Elements.Count;
        }

        public virtual string GetExpression { get; protected set; }

        public virtual ScriptableTask TaskSettings { get; protected set; }

        public virtual TaskBehaviour TaskBehaviour { get; protected set; }

        public virtual TaskType TaskType { get => TaskSettings.TaskType; }

        protected abstract System.Threading.Tasks.Task CreateTaskElementsAsync();

        public abstract UniTask CreateTaskView(Transform gameplayPanel);

        public abstract UniTask DisposeTaskView();

        protected abstract void SaveResult();


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (TaskBehaviour != null)
                    {
                        DisposeTaskView();
                    }

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Task()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            Dispose(false);
            #pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
            #pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            //Here we supposed to clear all resources
            if(TaskBehaviour != null)
            {
                await TaskBehaviour.DisposeAsync().ConfigureAwait(false);
            }
            //нужно переписать себе все диспоузы в елементвью и т.д на что то подобное
            //Elements[0].Dispose();
        }

        #endregion
    }
}