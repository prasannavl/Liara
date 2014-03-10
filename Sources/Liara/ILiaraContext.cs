// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

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
    public interface ILiaraContext : IDisposable
    {
        ILiaraEngine Engine { get; set; }
        ILiaraServerEnvironment Environment { get; }
        ILiaraRequest Request { get; }
        ILiaraResponse Response { get; }
        Dictionary<string, object> Items { get; set; }
        ILiaraServicesContainer Services { get; set; }
        Route Route { get; set; }
        ILiaraSecurity Security { get; set; }
        ILiaraLogWriter Log { get; }
        ILiaraLogWriter Trace { get; }
    }
}