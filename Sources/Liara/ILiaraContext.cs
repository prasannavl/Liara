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
    public interface ILiaraContext : IDisposable
    {
        ILiaraEngine Engine { get; set; }
        ILiaraServerEnvironment Environment { get; }
        ILiaraRequest Request { get; }
        ILiaraResponse Response { get; }
        Dictionary<string, object> Items { get; set; }
        ILiaraServicesContainer Services { get; set; }
        Route Route { get; set; }
        ILiaraLogWriter Log { get; }
    }
}