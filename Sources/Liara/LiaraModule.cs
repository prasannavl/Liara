// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using Liara.Internal;
using Liara.Logging;
using Liara.RequestProcessing;
using Liara.ResponseProcessing;
using Liara.Services;

namespace Liara
{

    [__DynamicallyInvokable]
    public abstract class LiaraModule
    {
        [__DynamicallyInvokable]
        public ILiaraContext Context { [__DynamicallyInvokable] get; [__DynamicallyInvokable] set; }

        public ILiaraRequest Request
        {
            get { return Context.Request; }
        }

        public ILiaraServicesContainer Services
        {
            get { return Context.Services; }
        }

        public ILiaraResponse Response
        {
            get { return Context.Response; }
        }

        public ILiaraLogWriter Log
        {
            get { return Context.Log; }
        }
    }
}