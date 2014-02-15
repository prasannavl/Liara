// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System;
using System.Threading.Tasks;

namespace Liara.MessageHandlers
{
    public class LiaraMessageHandler : LiaraMessageHandlerBase
    {
        private volatile bool disposed;
        private LiaraMessageHandlerBase innerHandler;

        private volatile bool operationStarted;

        public LiaraMessageHandlerBase InnerHandler
        {
            get { return innerHandler; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("innerHandler");
                CheckDisposedOrStarted();
                innerHandler = value;
            }
        }

        public override Task ProcessAsync(ILiaraContext context)
        {
            SetOperationStarted();
            return InnerHandler.ProcessAsync(context);
        }

        private void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        private void CheckDisposedOrStarted()
        {
            CheckDisposed();
            if (operationStarted)
                throw new InvalidOperationException("Inner handler association after operation start.");
        }

        private void SetOperationStarted()
        {
            CheckDisposed();
            if (innerHandler == null)
                throw new InvalidOperationException("Inner handler null.");
            if (operationStarted)
                return;
            operationStarted = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                disposed = true;
                if (innerHandler != null)
                    innerHandler.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}