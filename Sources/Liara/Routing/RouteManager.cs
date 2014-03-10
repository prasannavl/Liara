// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 6:47 PM 23-02-2014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Liara.Common;
using Liara.Helpers;
using Liara.MessageHandlers;
using Liara.Security;

namespace Liara.Routing
{
    using RouteMap = IDictionary<string, IDictionary<string, Route[]>>;

    public class RouteManager
    {
        private readonly ILiaraConfiguration configuration;
        private readonly LiaraExplorer explorer;

        public RouteManager(ILiaraConfiguration configuration)
        {
            this.configuration = configuration;
            explorer = new LiaraExplorer(configuration);
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
                    MethodInfo = aggRoute.ActionMethodInfo,
                    Handlers = aggRoute.Handlers,
                    RequestModel = aggRoute.RequestModel
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

                        route.ActionReturnType = LiaraActionReturnType.Void;

                        route.Action = new Action<ILiaraContext>(context =>
                        {
                            var module = configuration.Services.Get<LiaraModule>(moduleType.Name);
                            module.Context = context;
                            routeInvoke(module);
                        });
                    }
                    else
                    {
                        if (route.MethodInfo.ReturnType.IsSubclassOf(typeof (Task)))
                        {
                            var routeInvoke = Expression.Lambda<Func<object, Task<object>>>(callExp, m).Compile();
                            route.ActionReturnType = LiaraActionReturnType.Task;

                            route.Action = new Func<ILiaraContext, Task<object>>(context =>
                            {
                                var module = configuration.Services.Get<LiaraModule>(moduleType.Name);
                                module.Context = context;
                                return routeInvoke(module);
                            });
                        }
                        else
                        {
                            var routeInvoke = Expression.Lambda<Func<object, object>>(callExp, m).Compile();
                            route.ActionReturnType = LiaraActionReturnType.Generic;

                            route.Action = new Func<ILiaraContext, object>(context =>
                            {
                                var module = configuration.Services.Get<LiaraModule>(moduleType.Name);
                                module.Context = context;
                                return routeInvoke(module);
                            });
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException(
                        String.Format(
                            "\r\nMethods with paramters as routes are not yet supported. Use Request.Content to access the request data. For automatic static type de-serialization use RequestDto attribute.\r\n" +
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
                    if (Global.FrameworkLogger.IsEnabled)
                    {
                        Global.FrameworkLogger.WriteTo("Routes", "{0,-8} : {1} {2} {3}",
                            item,
                            string.IsNullOrWhiteSpace(route.Path) ? "/" : route.Path,
                            route.RequestModel == null ? string.Empty : " - In: " + route.RequestModel.Name,
                            route.ActionReturnType == LiaraActionReturnType.Task
                                ? " - Out : Task - " +
                                  string.Join(",",
                                      route.MethodInfo.ReturnType.GetGenericArguments().ToList().Select(x => x.FullName))
                                : " - Out : " + route.MethodInfo.ReturnType);
                    }

                    Route[] existingRoutes;
                    var routeList = routeDictionary.TryGetValue(item, out existingRoutes)
                        ? existingRoutes.ToList()
                        : new List<Route>();

                    routeList.Add(route);

                    // Sort by priority. Higher priority on top of the list.
                    routeList.Sort((r1, r2) => r2.Priority - r1.Priority);
                    routeDictionary[item] = routeList.ToArray();
                }

                routeMap[aggRoute.Path] = routeDictionary;
            }

            return routeMap;
        }

        public IEnumerable<AggregatedRoute> GetAggregatedRoutes(MethodInfo method)
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
                        Path = StringHelpers.CombineWebPath(routePrefix, currentRoute.Path),
                        Handlers = GetRouteHandlerCollection(method),
                        RequestModel = GetRouteRequestModel(method)
                    };

                    yield return newRoute;
                }
            }
        }

        private Type GetRouteRequestModel(MethodInfo method)
        {
            var reqModelAttrs = method.GetCustomAttributes(typeof (RequestModelAttribute), false);
            return reqModelAttrs.Any() ? reqModelAttrs.Cast<RequestModelAttribute>().First().RequestModel : null;
        }

        public IEnumerable<string> GetRoutePrefixes(MethodInfo method)
        {
            var routePrefixes = new List<string>();
            var declaringType = method.DeclaringType;
            if (declaringType != null)
            {
                var modulesInScope = explorer.GetLiaraModules();

                do
                {
                    var cPrefixes = declaringType.GetCustomAttributes(typeof (RoutePrefixAttribute), false);
                    if (cPrefixes.Length <= 0) continue;

                    if (!routePrefixes.Any())
                    {
                        routePrefixes.AddRange(from RoutePrefixAttribute prefix in cPrefixes select prefix.RoutePrefix);
                    }
                    else
                    {
                        foreach (RoutePrefixAttribute prefix in cPrefixes)
                        {
                            for (int i = 0; i < routePrefixes.Count; i++)
                            {
                                routePrefixes[i] = prefix.RoutePrefix + routePrefixes[i];
                            }
                        }
                    }
                } while ((declaringType = declaringType.DeclaringType) != null &&
                         modulesInScope.Any(m => m.GetType() == declaringType));
            }

            if (!routePrefixes.Any()) routePrefixes.Add("/");
            return routePrefixes;
        }

        public LiaraMessageHandlerCollection GetRouteHandlerCollection(MethodInfo method)
        {
            var routeHandlers = new LiaraMessageHandlerCollection();

            dynamic declaringType = method;
            if (declaringType != null)
            {
                var modulesInScope = explorer.GetLiaraModules();
                var allowAnonymous = false;
                do
                {
                    object[] handlers = declaringType.GetCustomAttributes(typeof (LiaraMessageHandler), false);
                    if (handlers.Length <= 0) continue;

                    foreach (
                        LiaraMessageHandler handler in handlers.OrderByDescending(h => ((LiaraMessageHandler) h).Order))
                    {
                        var type = handler.GetType();
                        if (allowAnonymous)
                        {
                            if (type == typeof (Authorize))
                            {
                                continue;
                            }
                        }
                        if (type == typeof (AllowAnonymous))
                        {
                            allowAnonymous = true;
                            continue;
                        }
                        routeHandlers.Insert(0, handler);
                    }
                } while ((declaringType = declaringType.DeclaringType) != null &&
                         modulesInScope.Any(m => m.GetType() == declaringType));

            }

            routeHandlers.Add((LiaraMessageHandler)explorer.GetConfiguration().ActionInvoker);
            return routeHandlers;
        }
    }
}