// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System.Collections.Generic;
using System.Threading.Tasks;
using Liara.Routing;
using RazorEngine.Templating;

namespace Liara.MessageHandlers
{
    public class RouteResolutionHandler : LiaraMessageHandler
    {
        public override Task ProcessAsync(ILiaraContext context)
        {
            var map = context.Engine.Configuration.Routes;
            var reqPath = context.Request.Info.Path;

            IDictionary<string, Route[]> routeList;
            if (map.TryGetValue(reqPath.TrimEnd('/'), out routeList))
            {
                Route[] routes;
                if (routeList.TryGetValue(context.Request.Info.Method, out routes))
                {
                    context.Route = routes[0];
                }
                else
                {
                    context.Response.Status = LiaraHttpStatus.MethodNotAllowed;
                    return TaskHelpers.Completed();
                }
            }
            else
            {
                context.Response.Status = LiaraHttpStatus.NotFound;
                return TaskHelpers.Completed();
            }

            return base.ProcessAsync(context);
        }
    }
}