// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:09 AM 16-02-2014

using System.Threading;
using System.Threading.Tasks;

namespace Liara.MessageHandlers
{
    public class LiaraThrottleHandler : LiaraMessageHandler
    {
        public static int MaxConcurrentRequests = 1000;

        private static readonly object SyncRoot = new object();
        private static int _requestsInProgress;

        public static int RequestsInProgress
        {
            get { return _requestsInProgress; }
        }

        public override Task ProcessAsync(ILiaraContext context)
        {
            lock (SyncRoot)
            {
                while (_requestsInProgress >= MaxConcurrentRequests)
                {
                    context.Log.Write("Max. concurrency level reached.");
                    context.Log.Write("Active Requests: " + _requestsInProgress);
                    Monitor.Wait(SyncRoot);
                }
                Interlocked.Increment(ref _requestsInProgress);
            }
            var res = base.ProcessAsync(context);
            lock (SyncRoot)
            {
                Interlocked.Decrement(ref _requestsInProgress);
                Monitor.Pulse(SyncRoot);
            }
            return res;
        }
    }
}