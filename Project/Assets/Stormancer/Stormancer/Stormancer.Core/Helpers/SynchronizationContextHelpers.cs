using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Stormancer
{
    public static class SynchronizationContextHelpers
    {
        /// <summary>
        /// same as <see cref="SynchronizationContext.Post(SendOrPostCallback, object)"/>, but uses <see cref="ThreadPool.QueueUserWorkItem(WaitCallback, object)"/> if the synchronization context is null
        /// </summary>
        /// <param name="context">The synchronization context on which to post the action. May be null.</param>
        /// <param name="callback">The callback to execute.</param>
        /// <param name="state">The state to transmit to the callback.</param>
        public static void SafePost(this SynchronizationContext context, SendOrPostCallback callback, object state)
        {
            if(context != null)
            {
                context.Post(callback, state);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(callback), state);
            }
        }

        /// <summary>
        /// same as <see cref="SynchronizationContext.Post(SendOrPostCallback, object)"/>, but uses <see cref="ThreadPool.QueueUserWorkItem(WaitCallback, object)"/> if the synchronization context is null
        /// </summary>
        /// <param name="context">The synchronization context on which to post the action. May be null.</param>
        /// <param name="callback">The callback to execute. Will be converted to a <see cref="SendOrPostCallback" internally./></param>
        public static void SafePost(this SynchronizationContext context, Action callback)
        {
            context.SafePost(_ => callback(), null);
        }
    }
}
