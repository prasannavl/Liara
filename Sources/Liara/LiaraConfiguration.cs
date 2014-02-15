// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Collections.Generic;
using System.Linq;
using Liara.Common;
using Liara.Formatting;
using Liara.Logging;
using Liara.MessageHandlers;
using Liara.ResponseProcessing;
using Liara.Routing;
using Liara.Services;

namespace Liara
{
    public class LiaraConfiguration : ILiaraConfiguration
    {
        private ILiaraFormatSelector formatSelector;
        private IList<ILiaraFormatter> formatters;
        private LiaraMessageHandlerCollection handlers;
        private bool isBuildComplete;
        private bool isDefaultHandlerListWired;
        private bool isFormatSelctorWired;
        private bool isFormatterListWired;
        private bool isFrameworkLoggerWired;
        private bool isLogWriterWired;
        private bool isResponseSynchronizerWired;
        private bool isRouteMapped;
        private bool isServicesContainerWired;
        private bool isStatusHandlerWired;
        private bool isTraceWriterWired;

        private ILiaraLogWriter logWriter;

        private ILiaraResponseSynchronizer responseSynchronizer;
        private IDictionary<string, IDictionary<string, Route[]>> routes;
        private bool serviceDiscoveryComplete;
        private ILiaraServicesContainer servicesContainer;
        private ILiaraStatusHandler statusHandler;
        private ILiaraLogWriter traceWriter;

        public LiaraConfiguration()
        {
            handlers = new LiaraMessageHandlerCollection();
            formatters = new List<ILiaraFormatter>();
        }

        public ILiaraServicesContainer Services
        {
            get { return servicesContainer; }
            set
            {
                servicesContainer = value;
                isServicesContainerWired = true;
            }
        }

        public bool UseBufferedRequest { get; set; }
        public bool UseBufferedResponse { get; set; }

        public IList<ILiaraFormatter> Formatters
        {
            get { return formatters; }
            set
            {
                formatters = value;
                isFormatterListWired = true;
            }
        }

        public ILiaraFormatSelector FormatSelector
        {
            get { return formatSelector; }
            set
            {
                formatSelector = value;
                isFormatSelctorWired = true;
            }
        }

        public ILiaraStatusHandler StatusHandler
        {
            get { return statusHandler; }
            set
            {
                statusHandler = value;
                isStatusHandlerWired = true;
            }
        }

        public IDictionary<string, IDictionary<string, Route[]>> Routes
        {
            get { return routes; }
            set
            {
                routes = value;
                isRouteMapped = true;
            }
        }

        public LiaraMessageHandlerCollection Handlers
        {
            get { return handlers; }
            set
            {
                handlers = value;
                isDefaultHandlerListWired = true;
            }
        }

        public ILiaraLogWriter LogWriter
        {
            get { return logWriter; }
            set
            {
                logWriter = value;
                isLogWriterWired = true;
            }
        }

        public ILiaraResponseSynchronizer ResponseSynchronizer
        {
            get { return responseSynchronizer; }
            set
            {
                responseSynchronizer = value;
                isResponseSynchronizerWired = true;
            }
        }

        public ILiaraLogWriter TraceWriter
        {
            get { return traceWriter; }
            set
            {
                traceWriter = value;
                isTraceWriterWired = true;
            }
        }

        public void Build()
        {
            try
            {
                if (!isBuildComplete)
                {
                    if (!isServicesContainerWired)
                        WireServicesContainer();
                    if (!serviceDiscoveryComplete)
                        DiscoverServices();
                    if (!isLogWriterWired)
                        WireLogWriter();
                    if (!isTraceWriterWired)
                        WireTraceWriter();
                    if (!isFrameworkLoggerWired)
                        WireFrameworkLogger();
                    if (!isRouteMapped)
                        WireRoutes();
                    if (!isDefaultHandlerListWired)
                        WireDefaultHandlers();
                    if (!isFormatterListWired)
                        WireFormatters();
                    if (!isFormatSelctorWired)
                        WireFormatSelector();
                    if (!isStatusHandlerWired)
                        WireStatusHandler();
                    if (!isResponseSynchronizerWired)
                        WireResponseSynchronizer();
                }

                if (Handlers.Last().InnerHandler == null)
                {
                    Handlers.Add(new TaskCompletionHandler());
                }

                isBuildComplete = true;
            }
            catch (Exception ex)
            {
                Global.FrameworkLogger.WriteException(ex, true);
            }
        }

        public virtual void WireServicesContainer()
        {
            servicesContainer = new DefaultServicesContainer();
            isServicesContainerWired = true;
        }

        public virtual void WireLogWriter()
        {
            var logger = Services.GetAll<ILiaraLogWriter>().OrderByDescending(x => x.Priority).FirstOrDefault();
            logWriter = logger ?? Global.FrameworkLogger;

            isLogWriterWired = true;
        }

        public virtual void WireDefaultHandlers()
        {
            Handlers.Insert(0, new RequestFormatHandler());
            Handlers.Insert(0, new RouteResolutionHandler());
            Handlers.Insert(0, new HttpStatusHandler());
            Handlers.Insert(0, new ResponseFormatHandler());
            if (UseBufferedResponse || UseBufferedRequest)
                Handlers.Insert(0, new BufferedStreamHandler());
#if DEBUG
            Handlers.Insert(0, new TraceHandler());
#endif

            Handlers.Insert(0, new ErrorHandler());
            Handlers.Insert(0, new LiaraThrottleHandler());

            Handlers.Add(new RouteInvocationHandler());
            isDefaultHandlerListWired = true;
        }


        public virtual void WireRoutes()
        {
            var routeMapper = new RouteManager(this);
            Routes = routeMapper.MapAll();
            isRouteMapped = true;
        }

        public virtual void WireFormatters()
        {
            var formatterList = Services.GetAll<ILiaraFormatter>()
                .Where(f => f.VerifyConfiguration())
                .OrderByDescending(f => f.Priority);

            if (!formatterList.Any()) formatters.Add(new RawFormatter());

            foreach (var liaraFormatter in formatterList)
            {
                formatters.Add(liaraFormatter);
            }

            isFormatterListWired = true;
        }

        public virtual void WireFormatSelector()
        {
            var selection =
                Services.GetAll<ILiaraFormatSelector>().OrderByDescending(x => x.Priority).FirstOrDefault() ??
                new LiaraFormatSelector();

            formatSelector = selection;
            isFormatSelctorWired = true;
        }

        public virtual void WireStatusHandler()
        {
            var selection =
                Services.GetAll<ILiaraStatusHandler>().OrderByDescending(x => x.Priority).FirstOrDefault() ??
                new LiaraStatusHandler();

            statusHandler = selection;
            isStatusHandlerWired = true;
        }

        public virtual void WireResponseSynchronizer()
        {
            var selection =
                Services.GetAll<ILiaraResponseSynchronizer>().OrderByDescending(x => x.Priority).FirstOrDefault() ??
                new EmptyResponseSynchronizer();

            responseSynchronizer = selection;
            isResponseSynchronizerWired = true;
        }

        public virtual void DiscoverServices()
        {
            var discoverer = new DefaultServiceDiscovery(Services);
            discoverer.Discover();
            serviceDiscoveryComplete = true;
        }

        private void WireFrameworkLogger()
        {
            if (traceWriter != null)
            {
                var isEnabled = Global.FrameworkLogger.IsEnabled;
                Global.FrameworkLogger = Services.Get<ILiaraLogWriter>(traceWriter.GetType().Name);
                Global.FrameworkLogger.IsEnabled = isEnabled;
            }

            isFrameworkLoggerWired = true;
        }

        private void WireTraceWriter()
        {
            if (traceWriter == null)
            {
                traceWriter = logWriter;
            }
            isTraceWriterWired = true;
        }
    }
}