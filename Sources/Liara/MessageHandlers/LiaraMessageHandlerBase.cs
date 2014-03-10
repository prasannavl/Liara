// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Threading.Tasks;
using Liara.Common;
using Liara.Formatting;

namespace Liara.MessageHandlers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class LiaraMessageHandlerBase : Attribute, IDisposable
    {
        public int Order { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract Task ProcessAsync(ILiaraContext request);

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}