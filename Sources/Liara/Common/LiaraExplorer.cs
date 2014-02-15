// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Liara.Formatting;
using Liara.Helpers;
using Liara.Routing;
using Liara.Services;

namespace Liara.Common
{
    public class LiaraExplorer
    {
        private readonly ILiaraServicesContainer container;

        public LiaraExplorer(ILiaraServicesContainer container)
        {
            this.container = container;
        }

        public IEnumerable<LiaraModule> GetLiaraModules()
        {
            return container.GetAll<LiaraModule>();
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
            return GetLiaraRouteMethods().SelectMany(GetLiaraRoutes);
        }

        public IEnumerable<AggregatedRoute> GetLiaraRoutes(MethodInfo method)
        {
            if (method.DeclaringType == null) yield break;

            var routePrefixes = GetRoutePrefixes(method);

            var routeAttrs = method.GetCustomAttributes(typeof (RouteAttribute), false);

            AggregatedRoute defaultRoute = null;
            var defaultRouteAdded = false;

            foreach (RouteAttribute routePathAttr in routeAttrs)
            {
                if (defaultRouteAdded && routePathAttr.Name == null)
                {
                    throw new ArgumentException(string.Format(
                        "Multiple routes defined without route names in {0} for {1}.",
                        method.DeclaringType.FullName,
                        method.Name));
                }

                var currentRoute = new AggregatedRoute {Path = routePathAttr.Path};

                // Add the single unnamed route as default route.
                if (!defaultRouteAdded && routePathAttr.Name == null)
                {
                    defaultRoute = currentRoute;
                    defaultRouteAdded = true;
                }

                // Resolve RouteMethodAttribute.

                var routeMethodAttrs = method.GetCustomAttributes(typeof (RouteMethodAttribute), false);
                {
                    AggregatedRoute route = null;
                    var setRouteMethod = false;

                    if (!routeMethodAttrs.Any())
                    {
                        currentRoute.RequestMethods.Add("GET");
                    }
                    foreach (RouteMethodAttribute item in routeMethodAttrs)
                    {
                        if (item.RouteName == null && defaultRouteAdded)
                        {
                            // Route unnamed. Use default route.
                            route = defaultRoute;
                            setRouteMethod = true;
                        }
                        else if (item.RouteName == routePathAttr.Name)
                        {
                            // Route is explicity named. Use the current route.
                            route = currentRoute;
                            setRouteMethod = true;
                        }

                        if (setRouteMethod)
                        {
                            var methods = item.RouteMethod.Where(m => !route.RequestMethods.Contains(m));
                            foreach (var m in methods)
                            {
                                route.RequestMethods.Add(m);
                            }
                        }
                    }
                }

                foreach (var routePrefix in routePrefixes)
                {
                    var newRoute = new AggregatedRoute
                    {
                        ActionMethodInfo = method,
                        RequestMethods = currentRoute.RequestMethods,
                        Priority = routePathAttr.Priority,
                        Path = StringHelpers.CombineWebPath(routePrefix, currentRoute.Path)
                    };

                    yield return newRoute;
                }
            }
        }

        public IEnumerable<string> GetRoutePrefixes(MethodInfo method)
        {
            var routePrefixes = new List<string>();
            var declaringType = method.DeclaringType;
            if (declaringType != null)
            {
                var cPrefixes = declaringType.GetCustomAttributes(typeof (RoutePrefixAttribute), false);

                if (cPrefixes.Length > 0)
                {
                    routePrefixes.AddRange(from RoutePrefixAttribute prefix in cPrefixes select prefix.RoutePrefix);

                    var modules = GetLiaraModules();

                    while ((declaringType = declaringType.DeclaringType) != null &&
                           modules.Any(m => m.GetType() == declaringType))
                    {
                        cPrefixes = declaringType.GetCustomAttributes(typeof (RoutePrefixAttribute), false);
                        if (cPrefixes.Length <= 0) continue;
                        foreach (RoutePrefixAttribute prefix in cPrefixes)
                        {
                            for (int i = 0; i < routePrefixes.Count; i++)
                            {
                                routePrefixes[i] = prefix.RoutePrefix + routePrefixes[i];
                            }
                        }
                    }
                }
            }
            if (!routePrefixes.Any()) routePrefixes.Add("/");
            return routePrefixes;
        }


        public IEnumerable<ILiaraFormatter> GetLiaraFormatters()
        {
            return container.GetAll<ILiaraFormatter>();
        }

        public IEnumerable<ILiaraFormatSelector> GetLiaraFormatSelectors()
        {
            return container.GetAll<ILiaraFormatSelector>();
        }

        public IEnumerable<ILiaraStatusHandler> GetLiaraStatusHandlers()
        {
            return container.GetAll<ILiaraStatusHandler>();
        }
    }
}