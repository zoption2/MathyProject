using System;
using System.Threading;
using System.Threading.Tasks;

//!!!! Add comments here
public static class DebounceExtention
{
    //Here is some real troubles, its not working as it supposed to and not waiting untill last change
    public static async System.Threading.Tasks.Task DebounceAsync<T, T1>(this Action<T, T1> func, int milliseconds = 5000)//was 300
    {
        await System.Threading.Tasks.Task.Delay(milliseconds)
           .ContinueWith(t => t, TaskScheduler.Default) ;
    }

    public static Action Debounce(this Action func, int milliseconds = 300)
    {
        CancellationTokenSource? cancelTokenSource = null;

        return () =>
        {
            cancelTokenSource?.Cancel();
            cancelTokenSource = new CancellationTokenSource();

            System.Threading.Tasks.Task.Delay(milliseconds, cancelTokenSource.Token)
                .ContinueWith(t =>
                {
                    if (t.IsCompletedSuccessfully)
                    {
                        func();
                    }
                }, TaskScheduler.Default);
        };
    }
    /// <summary>
    /// temp message, fill later
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    /// <param name="milliseconds">Amount of waiting time in milliseconds</param>
    /// <returns></returns>
    public static Action<T> Debounce<T>(this Action<T> func, int milliseconds = 300)
    {
        CancellationTokenSource? cancelTokenSource = null;

        return arg =>
        {
            cancelTokenSource?.Cancel();
            cancelTokenSource = new CancellationTokenSource();

            System.Threading.Tasks.Task.Delay(milliseconds, cancelTokenSource.Token)
                .ContinueWith(t =>
                {
                    if (t.IsCompletedSuccessfully)
                    {
                        func(arg);
                    }
                }, TaskScheduler.Default);
        };
    }

    //Not working as it supposed to
    // https://stackoverflow.com/questions/28472205/c-sharp-event-debounce/59296962#59296962  
    public static Action<T, T1> Debounce<T, T1>(this Action<T, T1> func, int milliseconds = 5000)
    {
        CancellationTokenSource lastCToken = null;

        Action<T, T1> temp = (arg1, arg2) =>
        {
            UnityEngine.Debug.Log("Is this debounce trash even work?!");
            //Cancel/dispose previous
            lastCToken?.Cancel();
            try
            {
                lastCToken?.Dispose();
            }
            catch { }

            var tokenSrc = lastCToken = new CancellationTokenSource();

            System.Threading.Tasks.Task.Delay(milliseconds).
            ContinueWith(task =>
            {
                UnityEngine.Debug.Log("Debounce should do the method");
                func(arg1, arg2);

            }, tokenSrc.Token);
        };

        return temp;
        /*
        return (arg1, arg2) =>
        {
            UnityEngine.Debug.Log("Is this debounce trash even work?!");
            //Cancel/dispose previous
            lastCToken?.Cancel();
            try
            {
                lastCToken?.Dispose();
            }
            catch { }

            var tokenSrc = lastCToken = new CancellationTokenSource();

            System.Threading.Tasks.Task.Delay(milliseconds).
            ContinueWith(task => 
            {
                UnityEngine.Debug.Log("Debounce should do the method");
                func(arg1, arg2);

            }, tokenSrc.Token);
        };
        /*
        CancellationTokenSource? cancelTokenSource = null;

        return (arg1, arg2) =>
        {
            cancelTokenSource?.Cancel();
            cancelTokenSource = new CancellationTokenSource();

            System.Threading.Tasks.Task.Delay(milliseconds, cancelTokenSource.Token)
                .ContinueWith(t =>
                {
                    if (t.IsCompletedSuccessfully)
                    {
                        UnityEngine.Debug.Log("Debounce should do the method");

                        func(arg1, arg2);
                    }
                }, TaskScheduler.Default);
        }; 
        */
    }

    /*
    //Debounce with 2 arguments

    public static Action<T, T1> Debounce<T, T1>(this Action<T, T1> func, int milliseconds = 5000)
    {
        CancellationTokenSource? cancelTokenSource = null;

        return (arg1, arg2) =>
        {
            cancelTokenSource?.Cancel();
            cancelTokenSource = new CancellationTokenSource();

            System.Threading.Tasks.Task.Delay(milliseconds, cancelTokenSource.Token)
                .ContinueWith(t =>
                {
                    if (t.IsCompletedSuccessfully)
                    {
                        func(arg1, arg2);
                    }
                }, TaskScheduler.Default);
        };
    }
    */

}

