// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 6:47 PM 23-02-2014

using System;
using System.Collections.Generic;
using Liara.Logging;
using Liara.RequestProcessing;
using Liara.ResponseProcessing;
using Liara.Routing;
using Liara.Security;
using Liara.Services;

namespace Liara
{
    public class LiaraContext : ILiaraContext
    {
        private bool disposed;
        private Dictionary<string, object> items;
        private ILiaraServicesContainer services;

        public LiaraContext(ILiaraServerEnvironment environment)
        {
            Environment = environment;
            Request = new LiaraRequest(this);
            Response = new LiaraResponse(this);
            Security = new LiaraSecurity();
        }

        public LiaraContext(ILiaraEngine engine, ILiaraServerEnvironment environment) : this(environment)
        {
            Engine = engine;
        }

        public ILiaraEngine Engine { get; set; }
        public ILiaraServerEnvironment Environment { get; private set; }
        public ILiaraRequest Request { get; set; }
        public ILiaraResponse Response { get; set; }

        public Dictionary<string, object> Items
        {
            get { return items ?? (items = new Dictionary<string, object>()); }
            set { items = value; }
        }

        public ILiaraServicesContainer Services
        {
            get { return services ?? (services = Engine.Configuration.Services.GetChildScope()); }
            set { services = value; }
        }

        public Route Route { get; set; }

        public ILiaraLogWriter Log
        {
            get { return Engine.Configuration.LogWriter; }
        }

        public ILiaraLogWriter Trace
        {
            get { return Engine.Configuration.TraceWriter; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public ILiaraSecurity Security { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                disposed = true;
                if (Services != null)
                {
                    Services.Dispose();
                }

                Environment.Dispose();
            }
        }
    }
}