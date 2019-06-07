using System.Collections.Generic;
using UniRx;

namespace System.Threading.Tasks
{

    public static class TaskHelper
    {
        private static Task _completed;
        public static Task Completed
        {
            get
            {
                if (_completed == null)
                {
                    var tce = new TaskCompletionSource<bool>();
                    _completed = tce.Task;
                    tce.SetResult(true);
                }
                return _completed;
            }
        }

        public static Task<T> FromResult<T>(T result)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
        }

        public static Task FromException(Exception ex)
        {
            return FromException<bool>(ex);
        }

        public static Task<T> FromException<T>(Exception ex)
        {
            var tcs = new TaskCompletionSource<T>();

            tcs.SetException(ex);

            return tcs.Task;
        }

        public static Task<T> FromExceptions<T>(IEnumerable<Exception> exceptions)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(exceptions);
            return tcs.Task;
        }

        public static Task If(bool condition, Func<Task> action)
        {
            if (condition)
            {
                return action();
            }
            else
            {
                return TaskHelper.FromResult(true);
            }
        }

        public static Task<T> IfElse<T>(bool condition, Func<Task<T>> conditionTrue, Func<Task<T>> conditionFalse)
        {
            if (condition)
            {
                return conditionTrue();
            }
            else
            {
                return conditionFalse();
            }
        }

        public static Task Delay(int milliseconds)
        {
            return Delay(TimeSpan.FromMilliseconds(milliseconds));
        }

        public static Task Delay(TimeSpan delay)
        {
            var tcs = new TaskCompletionSource<Unit>();

            Timer timer = null;
            timer = new Timer(_ =>
            {
                tcs.SetResult(Unit.Default);
                timer.Dispose();
            }, null, delay, TimeSpan.FromMilliseconds(-1));


            return tcs.Task;
        }
    }

}
