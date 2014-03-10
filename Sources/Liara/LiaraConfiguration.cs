// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 6:47 PM 23-02-2014

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Liara.Common;
using Liara.Formatting;
using Liara.Logging;
using Liara.MessageHandlers;
using Liara.ResponseProcessing;
using Liara.Routing;
using Liara.Security;
using Liara.Services;

namespace Liara
{
    public class LiaraConfiguration : ILiaraConfiguration
    {
        private ILiaraActionInvoker actionInvoker;
        private ILiaraAuthenticationHandler authenticationHandler;
        private ILiaraFormatSelector formatSelector;
        private LiaraFormatterCollection formatters;
        private LiaraMessageHandlerCollection handlers;
        private bool isActionInvokerWired;
        private bool isAuthenticationHandlerWired;
        private bool isBuildComplete;
        private bool isDefaultHandlerListWired;
        private bool isFormatSelctorWired;
        private bool isFormatterListWired;
        private bool isFrameworkLoggerWired;
        private bool isInitialized;
        private bool isLogWriterWired;
        private bool isResponseSynchronizerWired;
        private bool isRouteMapped;
        private bool isServicesContainerWired;
        private bool isStatusHandlerWired;
        private bool isTraceWriterWired;

        private ILiaraLogWriter logWriter;

        private ILiaraResponseSynchronizer responseSynchronizer;
        private string rootDirectory;
        private IDictionary<string, IDictionary<string, Route[]>> routes;
        private bool serviceDiscoveryComplete;
        private ILiaraServicesContainer servicesContainer;
        private ILiaraStatusHandler statusHandler;
        private ILiaraLogWriter traceWriter;

        public LiaraConfiguration()
        {
            handlers = new LiaraMessageHandlerCollection();
            formatters = new LiaraFormatterCollection();
            Items = new LiaraHashTable<object>(isCaseSensitive: true);
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

        public LiaraFormatterCollection Formatters
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

        public ILiaraAuthenticationHandler AuthenticationHandler
        {
            get { return authenticationHandler; }
            set
            {
                authenticationHandler = value;
                isAuthenticationHandlerWired = true;
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

        public ILiaraHashTable<object> Items { get; set; }

        public string RootDirectory
        {
            get
            {
                return rootDirectory ?? (rootDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            }
            set { rootDirectory = value; }
        }

        public ILiaraActionInvoker ActionInvoker
        {
            get { return actionInvoker; }
            set
            {
                actionInvoker = value;
                isActionInvokerWired = true;
            }
        }

        public void Build()
        {
            try
            {
                if (!isInitialized)
                {
                    Initialize();
                }
                if (!isBuildComplete)
                {
                    if (!isLogWriterWired)
                        WireLogWriter();
                    if (!isTraceWriterWired)
                        WireTraceWriter();
                    if (!isFrameworkLoggerWired)
                        WireFrameworkLogger();
                    if (!isActionInvokerWired)
                        WireActionInvoker();
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
                    if (!isAuthenticationHandlerWired)
                        WireAuthenticationHandler();
                    if (!isResponseSynchronizerWired)
                        WireResponseSynchronizer();

                    LogConfiguration();
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
            // Insert pre-handlers in the reverse order. 
            Handlers.Insert(0, new RequestFormatHandler());
            Handlers.Insert(0, new RouteResolutionHandler());
            Handlers.Insert(0, new HttpStatusHandler());
            Handlers.Insert(0, new ResponseFormatHandler());
#if DEBUG
            Handlers.Insert(0, new TraceHandler());
#endif
            Handlers.Insert(0, new ErrorHandler());

            // Buffered stream handler has to be after error handling so that buffered streams can be copied back.
            if (UseBufferedResponse || UseBufferedRequest)
                Handlers.Insert(0, new BufferedStreamHandler());

            Handlers.Insert(0, new ThrottleHandler());

            // Add the final handlers after the user handlers.
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

        public void Initialize()
        {
            if (!isInitialized)
            {
                if (!isServicesContainerWired)
                    WireServicesContainer();
                if (!serviceDiscoveryComplete)
                    DiscoverServices();
            }
            isInitialized = true;
        }

        public void LogConfiguration()
        {
            if (Global.FrameworkLogger.IsEnabled)
            {
                foreach (var prop in typeof (ILiaraConfiguration).GetProperties())
                {
                    try
                    {
                        if (prop.Name == "Routes") continue;

                        var value = prop.GetValue(this);
                        var type = value.GetType();
                        if (type == typeof (LiaraMessageHandlerCollection))
                        {
                            var values = (from handler in Handlers select handler.GetType().Name).ToList();
                            Global.FrameworkLogger.WriteTo("Configuration", "\r\n{0}\r\n{1}",
                                prop.Name + "\r\n" + new string('-', prop.Name.Length),
                                "\r\n" + String.Join("\r\n", values) + "\r\n");
                        }
                        else if (type.IsGenericType && type.GetGenericTypeDefinition().Name == "List`1")
                        {
                            var valueList = (IList) value;
                            var values = (from object item in valueList select item.GetType().Name).ToList();

                            Global.FrameworkLogger.WriteTo("Configuration", "\r\n{0}\r\n{1}",
                                prop.Name + "\r\n" + new string('-', prop.Name.Length),
                                "\r\n" + String.Join("\r\n", values) + "\r\n");
                        }
                        else
                        {
                            Global.FrameworkLogger.WriteTo("Configuration", "{0,-22} : {1}", prop.Name, value);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public virtual void WireFrameworkLogger()
        {
            if (traceWriter != null)
            {
                var isEnabled = Global.FrameworkLogger.IsEnabled;
                Global.FrameworkLogger = Services.Get<ILiaraLogWriter>(traceWriter.GetType().Name);
                Global.FrameworkLogger.IsEnabled = isEnabled;
            }

            isFrameworkLoggerWired = true;
        }

        public virtual void WireTraceWriter()
        {
            if (traceWriter == null)
            {
                traceWriter = logWriter;
            }
            isTraceWriterWired = true;
        }

        public virtual void WireActionInvoker()
        {
            var selection =
                Services.GetAll<ILiaraActionInvoker>().FirstOrDefault() ??
                new ActionInvocationHandler();
            actionInvoker = selection;
            isActionInvokerWired = true;
        }

        public virtual void WireAuthenticationHandler()
        {
            var selection =
                Services.GetAll<ILiaraAuthenticationHandler>().OrderByDescending(x => x.Priority).FirstOrDefault() ??
                new LiaraAuthenticationHandler();

            authenticationHandler = selection;
            isAuthenticationHandlerWired = true;
        }
    }
}