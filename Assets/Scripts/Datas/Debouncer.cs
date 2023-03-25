using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


namespace Mathy.Data
{
    //Not working 
    public class Debouncer : IDisposable
    {
        //Debounce constructor not supported, so I need to create it and call some SET method
        public Debouncer()
        {
            debounceCancellationTokenSource = new CancellationTokenSource();
        }

        //Hmm its possible to store like this
        private System.Object debounceAction;
        //How much time will wait debouncer before action, in milliseconds, by default its 3s 
        public int DebounceInterval { get; set; } = 7000;
        private CancellationTokenSource debounceCancellationTokenSource;

        public void SetAction<T,T1>(Action<T, T1> action)
        {
            debounceAction = action;
            Start<T, T1>();
        }

        public void Start<T,T1>()
        {
            Debug.Log("Obj stored type: " + debounceAction.GetType());
            var action = (Action<T, T1>)debounceAction;
            action.Debounce();
        }

        public void Dispose()
        {
            //Clearing all data and stop all trash
            throw new NotImplementedException();
        }
    }

    class Debouncer2
    {
        private List<CancellationTokenSource> StepperCancelTokens = new List<CancellationTokenSource>();
        private int MillisecondsToWait;
        private readonly object _lockThis = new object(); // Use a locking object to prevent the debouncer to trigger again while the func is still running

        public Debouncer2(int millisecondsToWait = 300)
        {
            this.MillisecondsToWait = millisecondsToWait;
        }

        public void Debouce(Action action)
        {
            CancelAllStepperTokens(); // Cancel all api requests;
            var newTokenSrc = new CancellationTokenSource();
            lock (_lockThis)
            {
                StepperCancelTokens.Add(newTokenSrc);
            }
            System.Threading.Tasks.Task.Delay(MillisecondsToWait, newTokenSrc.Token).ContinueWith(task => // Create new request
            {
                if (!newTokenSrc.IsCancellationRequested) // if it hasn't been cancelled
                {
                    CancelAllStepperTokens(); // Cancel any that remain (there shouldn't be any)
                    StepperCancelTokens = new List<CancellationTokenSource>(); // set to new list
                    lock (_lockThis)
                    {
                        action(); // running the function
                    }
                }
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        //Not working yet
        public void Debouce<T,T1>(Action<T, T1> action, T param1, T1 param2)
        {
            CancelAllStepperTokens(); // Cancel all api requests;
            var newTokenSrc = new CancellationTokenSource();
            lock (_lockThis)
            {
                StepperCancelTokens.Add(newTokenSrc);
            }
            System.Threading.Tasks.Task.Delay(MillisecondsToWait, newTokenSrc.Token).ContinueWith(task => // Create new request
            {
                if (!newTokenSrc.IsCancellationRequested) // if it hasn't been cancelled
                {
                    CancelAllStepperTokens(); // Cancel any that remain (there shouldn't be any)
                    StepperCancelTokens = new List<CancellationTokenSource>(); // set to new list
                    lock (_lockThis)
                    {
                        action(param1, param2);
                    }
                }
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void CancelAllStepperTokens()
        {
            foreach (var token in StepperCancelTokens)
            {
                if (!token.IsCancellationRequested)
                {
                    token.Cancel();
                }
            }
        }
    }

}