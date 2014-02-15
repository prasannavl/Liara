// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Liara.Common;
using Liara.Services;

namespace Liara.Routing
{
    using RouteMap = IDictionary<string, IDictionary<string, Route[]>>;

    public class RouteMapper
    {
        private readonly LiaraExplorer explorer;
        private readonly ILiaraConfiguration configuration;

        public RouteMapper(ILiaraConfiguration config)
        {
            this.configuration = config;
            explorer = new LiaraExplorer(config.Services);
        }

        public RouteMap MapAll()
        {
            return Map(explorer.GetLiaraRoutes());
        }

        // TODO: Switch to Trie-based routing for better performance.
        public RouteMap Map(IEnumerable<AggregatedRoute> routes)
        {
            var routeMap = new Dictionary<string, IDictionary<string, Route[]>>(StringComparer.OrdinalIgnoreCase);
            foreach (var aggRoute in routes)
            {
                var route = new Route
                {
                    Path = aggRoute.Path,
                    Priority = aggRoute.Priority,
                    MethodInfo = aggRoute.ActionMethodInfo
                };

                if (route.MethodInfo.DeclaringType == null)
                    throw new ArgumentNullException("route->" + "MethodInfo->DeclaringType");

                var moduleType = route.MethodInfo.DeclaringType;

                // Set up Expression Tree.
                var m = Expression.Parameter(typeof (object));

                if (route.MethodInfo.GetParameters().Length == 0)
                {
                    var callExp = Expression.Call(Expression.Convert(m, moduleType), route.MethodInfo);

                    if (route.MethodInfo.ReturnType == typeof (void))
                    {
                        var routeInvoke = Expression.Lambda<Action<object>>(callExp, m).Compile();
                        
                        route.Action = new Action<ILiaraContext>(context =>
                        {

                            var module = configuration.Services.Get<LiaraModule>(moduleType.Name);
                            module.Context = context;
                            routeInvoke(module);
                        });
                    }
                    else
                    {
                        var routeInvoke = Expression.Lambda<Func<object, object>>(callExp, m).Compile();

                        route.Action = new Func<ILiaraContext, object>(context =>
                        {

                            var module = configuration.Services.Get<LiaraModule>(moduleType.Name);
                            module.Context = context;
                            return routeInvoke(module);
                        });
                    }
                }
                else
                {
                    throw new NotSupportedException(
                       String.Format("\r\nMethods with paramters as routes are not yet supported. Use Request.Content to access the request data. For automatic static type de-serialization use RequestDto attribute.\r\n" +
                                     "Method: {0}\r\n" + 
                                     "Type: {1}\r\n", 
                                     route.MethodInfo.Name,
                                     moduleType.FullName));
                }


                IDictionary<string, Route[]> routeDictionary;

                if (!routeMap.TryGetValue(aggRoute.Path, out routeDictionary))
                {
                    routeDictionary = new Dictionary<string, Route[]>(StringComparer.Ordinal);
                }

                foreach (var item in aggRoute.RequestMethods)
                {
                    Route[] existingRoutes;
                    List<Route> routeList;
                    if (routeDictionary.TryGetValue(item, out existingRoutes))
                    {
                        routeList = existingRoutes.ToList();
                    }
                    else
                    {
                        routeList = new List<Route>();
                    }
                    routeList.Add(route);

                    // Sort by priority. Higher priority on top of the list.
                    routeList.Sort((r1, r2) => r2.Priority - r1.Priority);
                    routeDictionary[item] = routeList.ToArray();
                }

                routeMap[aggRoute.Path] = routeDictionary;
            }

            return routeMap;
        }
    }
}