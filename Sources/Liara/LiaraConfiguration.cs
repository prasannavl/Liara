// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private bool isLogWriterWired;
        private bool isResponseSynchronizerWired;
        private bool isRouteMapped;
        private bool isServicesContainerWired;
        private bool isStatusHandlerWired;

        private ILiaraLogWriter logWriter;

        private ILiaraResponseSynchronizer responseSynchronizer;
        private IDictionary<string, IDictionary<string, Route[]>> routes;
        private bool serviceDiscoveryComplete;
        private ILiaraServicesContainer servicesContainer;
        private ILiaraStatusHandler statusHandler;

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

        public void Build()
        {
            try
            {
                if (!isBuildComplete)
                {
                    if (!isServicesContainerWired)
                        WireServicesContainer();
                    if (!serviceDiscoveryComplete)
                        Services.Discover();
                    if (!isLogWriterWired)
                        WireLogWriter();
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
                LiaraEngine.FrameworkLogger.WriteException(ex, true);
            }
        }

        public void WireServicesContainer()
        {
            servicesContainer = new DefaultServicesContainer();
            isServicesContainerWired = true;
        }

        public void WireLogWriter()
        {
            var logger = Services.GetAll<ILiaraLogWriter>().OrderByDescending(x => x.Priority).FirstOrDefault();
            logWriter = logger ?? LiaraEngine.FrameworkLogger;

            isLogWriterWired = true;
        }

        public void WireDefaultHandlers()
        {
            Handlers.Insert(0, new RequestFormatHandler());
            Handlers.Insert(0, new RouteResolutionHandler());
            Handlers.Insert(0, new HttpStatusHandler());
            Handlers.Insert(0, new ResponseFormatHandler());
            if (UseBufferedResponse || UseBufferedRequest)
                Handlers.Insert(0, new BufferedStreamHandler());
            Handlers.Insert(0, new ErrorHandler());

            Handlers.Add(new RouteInvocationHandler());
            isDefaultHandlerListWired = true;
        }


        public void WireRoutes()
        {
            var routeMapper = new RouteMapper(this);
            Routes = routeMapper.MapAll();
            isRouteMapped = true;
        }

        public void WireFormatters()
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

        public void WireFormatSelector()
        {
            var selection =
                Services.GetAll<ILiaraFormatSelector>().OrderByDescending(x => x.Priority).FirstOrDefault() ??
                new LiaraFormatSelector();

            formatSelector = selection;
            isFormatSelctorWired = true;
        }

        public void WireStatusHandler()
        {
            var selection =
                Services.GetAll<ILiaraStatusHandler>().OrderByDescending(x => x.Priority).FirstOrDefault() ??
                new LiaraStatusHandler();

            statusHandler = selection;
            isStatusHandlerWired = true;
        }

        public void WireResponseSynchronizer()
        {
            var selection =
                Services.GetAll<ILiaraResponseSynchronizer>().OrderByDescending(x => x.Priority).FirstOrDefault() ??
                new EmptyResponseSynchronizer();

            responseSynchronizer = selection;
            isResponseSynchronizerWired = true;
        }

        public void DiscoverServices()
        {
            Services.Discover();
            serviceDiscoveryComplete = true;
        }
    }
}