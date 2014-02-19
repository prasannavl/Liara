// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

using System.Threading;
using System.Threading.Tasks;

namespace Liara.MessageHandlers
{
    public class LiaraThrottleHandler : LiaraMessageHandler
    {
        private readonly object syncRoot = new object();
        public int MaxConcurrentRequests = 1000;
        private int requestsInProgress;

        public int RequestsInProgress
        {
            get { return requestsInProgress; }
        }

        public override Task ProcessAsync(ILiaraContext context)
        {
            lock (syncRoot)
            {
                while (requestsInProgress >= MaxConcurrentRequests)
                {
                    context.Log.WriteAsync("Max. concurrency level reached.");
                    context.Log.WriteAsync("Active Requests: " + requestsInProgress);
                    Monitor.Wait(syncRoot);
                }
                Interlocked.Increment(ref requestsInProgress);
            }
            var res = base.ProcessAsync(context);
            lock (syncRoot)
            {
                Interlocked.Decrement(ref requestsInProgress);
                Monitor.Pulse(syncRoot);
            }
            return res;
        }
    }
}