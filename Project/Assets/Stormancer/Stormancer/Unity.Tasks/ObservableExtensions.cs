using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;

namespace System.Threading.Tasks
{
    public static class ObservableExtensions
    {
        /// <summary>
        /// Converts the IObservable to a Task.
        /// </summary>
        /// <typeparam name="T">The return type of the source IObservable.</typeparam>
        /// <param name="source">The observable to convert.</param>
        /// <returns>A task completing or getting faulted at the same time as the observable.</returns>
        /// <remarks>The tasks completes when the converted observable completes, regardless of how many elements it had before completing.</remarks>
        public static Task ToVoidTask<T>(this IObservable<T> source, CancellationToken token)
        {
            return SubscribeAndCleanUp<T, Unit>(source,
                (IObservable<T> obs,TaskCompletionSource<Unit>  tcs) => obs.Subscribe(
                    (T t) => { },
                    (Exception ex) => tcs.SetException(ex),
                    () => tcs.SetResult(Unit.Default)), token);
        }

        private static async Task<TResult> SubscribeAndCleanUp<TData, TResult>(
            IObservable<TData> observable,
            Func<IObservable<TData>, TaskCompletionSource<TResult>, IDisposable> subscriptionMethod, CancellationToken token)
        {
         
            var tcs = new TaskCompletionSource<TResult>();

            var subscription = subscriptionMethod(observable, tcs);

            IDisposable tokenRegistration=null;

            token.Register(() =>
            {
                if (subscription != null)
                {
                    subscription.Dispose();
                    subscription = null;
                }
                if (tokenRegistration != null)
                {
                    tokenRegistration.Dispose();
                    tokenRegistration = null;
                }
            });

            var result = await tcs.Task;
            if (subscription != null)
            {
                subscription.Dispose();
                subscription = null;
            }
            if (tokenRegistration != null)
            {
                tokenRegistration.Dispose();
                tokenRegistration = null;
            }
            return result;
        }
    }
}
