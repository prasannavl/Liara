// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System;
using System.Collections.Generic;
using System.Linq;

namespace Liara
{
    /// <summary>
    ///     Route prefix to be added to LiaraModules.
    ///     <para>Note: Must start with a forward slash.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RoutePrefixAttribute : Attribute
    {
        public RoutePrefixAttribute(string routePrefix)
        {
            if (String.IsNullOrWhiteSpace(routePrefix))
            {
                throw new ArgumentException("Route prefix cannot be empty");
            }
            if (!routePrefix.StartsWith("/"))
            {
                throw new ArgumentException(string.Format(
                    "Route prefix of '{0}' must start with a forward slash '/'.",
                    routePrefix));
            }

            RoutePrefix = routePrefix.TrimEnd('/');
        }

        public string RoutePrefix { get; set; }
    }

    /// <summary>
    ///     Route path, for LiaraModule methods.
    ///     <para>
    ///         Route names are necessary if more than one route is specified for a method.
    ///     </para>
    ///     <para>Note: Must start with a forward slash.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RouteAttribute : Attribute
    {
        public RouteAttribute(string routePath, string routeName = null, int priority = 0)
        {
            if (String.IsNullOrWhiteSpace(routePath))
            {
                throw new ArgumentException("Route path cannot be empty");
            }
            if (!routePath.StartsWith("/"))
            {
                throw new ArgumentException(string.Format(
                    "Route path value of '{0}' must start with a forward slash '/'.",
                    routePath));
            }

            Name = routeName;
            Path = routePath;
            Priority = priority;
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public int Priority { get; set; }
    }

    /// <summary>
    ///     The HttpMethods for the route, as comma seperated string.
    ///     <para>
    ///         If a route name is not specified, it applies to the default un-named route, if present.
    ///     </para>
    ///     <example>
    ///         [RouteMethod("GET, POST")]
    ///         [RouteMethod("GET, HEAD, POST", "route1")]
    ///     </example>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RouteMethodAttribute : Attribute
    {
        public RouteMethodAttribute(string routeMethods, string routeName = null)
        {
            RouteName = routeName;
            RouteMethod =
                routeMethods.Split(',').Select(s => s.Trim().ToUpperInvariant()).Distinct().ToList();
        }

        public string RouteName { get; set; }
        public IList<string> RouteMethod { get; set; }
    }

    /// <summary>
    ///     Route conditon.
    ///     <para>Route will be matched only if the conditon predicate is satisfied.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RouteConditionAttribute : Attribute
    {
        public RouteConditionAttribute(string routeCondition, string routeName = null)
        {
            RouteName = routeName;
            RouteCondition = routeCondition;
        }

        public string RouteName { get; set; }
        public string RouteCondition { get; set; }
    }
}