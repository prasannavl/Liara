// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System;
using System.Collections.Generic;
using Liara.Logging;
using Liara.RequestProcessing;
using Liara.ResponseProcessing;
using Liara.Routing;
using Liara.Services;

namespace Liara
{
    public class LiaraContext : ILiaraContext
    {
        private bool disposed;
        private ILiaraServicesContainer services;

        public LiaraContext(ILiaraServerEnvironment environment)
        {
            Environment = environment;
            Items = new Dictionary<string, object>();
            Request = new LiaraRequest(this);
            Response = new LiaraResponse(this);
        }

        public LiaraContext(ILiaraEngine engine, ILiaraServerEnvironment environment) : this(environment)
        {
            Engine = engine;
        }

        public ILiaraEngine Engine { get; set; }
        public ILiaraServerEnvironment Environment { get; private set; }
        public ILiaraRequest Request { get; set; }
        public ILiaraResponse Response { get; set; }
        public Dictionary<string, object> Items { get; set; }

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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