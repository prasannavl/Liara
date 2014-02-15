// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 1:24 AM 15-02-2014

using System.Collections.Generic;
using Liara.Common;
using Liara.Formatting;
using Liara.Logging;
using Liara.MessageHandlers;
using Liara.ResponseProcessing;
using Liara.Routing;
using Liara.Services;

namespace Liara
{
    public interface ILiaraConfiguration
    {
        ILiaraServicesContainer Services { get; set; }
        bool UseBufferedRequest { get; set; }
        bool UseBufferedResponse { get; set; }
        IList<ILiaraFormatter> Formatters { get; set; }
        ILiaraFormatSelector FormatSelector { get; set; }
        ILiaraStatusHandler StatusHandler { get; set; }
        IDictionary<string, IDictionary<string, Route[]>> Routes { get; set; }
        LiaraMessageHandlerCollection Handlers { get; set; }
        ILiaraLogWriter LogWriter { get; set; }
        ILiaraResponseSynchronizer ResponseSynchronizer { get; set; }
        void Build();
        void WireServicesContainer();
        void WireLogWriter();
        void WireDefaultHandlers();
        void WireRoutes();
        void WireFormatters();
        void WireFormatSelector();
        void WireStatusHandler();
        void WireResponseSynchronizer();
        void DiscoverServices();
    }
}