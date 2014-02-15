// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Liara.Formatting;
using Liara.Helpers;
using Liara.Routing;

namespace Liara.Common
{
    public class LiaraExplorer
    {
        private readonly ILiaraConfiguration configuration;

        public LiaraExplorer(ILiaraConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IEnumerable<LiaraModule> GetLiaraModules()
        {
            return configuration.Services.GetAll<LiaraModule>();
        }


        public IEnumerable<MethodInfo> GetLiaraRouteMethods()
        {
            return from mod in GetLiaraModules()
                from method in ReflectionHelpers.GetMethods(mod.GetType())
                where method.IsDefined(typeof (RouteAttribute), true)
                select method;
        }

        public IEnumerable<AggregatedRoute> GetLiaraRoutes()
        {
            return GetLiaraRouteMethods().SelectMany(new RouteManager(configuration).GetAggregatedRoutes);
        }

        public IEnumerable<ILiaraFormatter> GetLiaraFormatters()
        {
            return configuration.Services.GetAll<ILiaraFormatter>();
        }

        public IEnumerable<ILiaraFormatSelector> GetLiaraFormatSelectors()
        {
            return configuration.Services.GetAll<ILiaraFormatSelector>();
        }

        public IEnumerable<ILiaraStatusHandler> GetLiaraStatusHandlers()
        {
            return configuration.Services.GetAll<ILiaraStatusHandler>();
        }
    }
}