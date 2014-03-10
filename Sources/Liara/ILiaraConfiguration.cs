using System.Collections.Generic;
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
    public interface ILiaraConfiguration
    {
        ILiaraServicesContainer Services { get; set; }
        bool UseBufferedRequest { get; set; }
        bool UseBufferedResponse { get; set; }
        LiaraFormatterCollection Formatters { get; set; }
        ILiaraFormatSelector FormatSelector { get; set; }
        ILiaraAuthenticationHandler AuthenticationHandler { get; set; }
        ILiaraStatusHandler StatusHandler { get; set; }
        IDictionary<string, IDictionary<string, Route[]>> Routes { get; set; }
        LiaraMessageHandlerCollection Handlers { get; set; }
        ILiaraLogWriter LogWriter { get; set; }
        ILiaraResponseSynchronizer ResponseSynchronizer { get; set; }
        ILiaraActionInvoker ActionInvoker { get; set; }
        ILiaraLogWriter TraceWriter { get; set; }
        ILiaraHashTable<object> Items { get; set; }
        string RootDirectory { get; set; }
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
        void Initialize();
        void LogConfiguration();
        void WireFrameworkLogger();
        void WireTraceWriter();
    }
}