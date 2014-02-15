// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Threading.Tasks;
using Liara.Routing;

namespace Liara.MessageHandlers
{
    /// <summary>
    ///     Route invocation handler. Usually the last in the handler chain.
    ///     Calls the route specific handlers, and the route action, typically inside the LiaraModule.
    /// </summary>
    public class RouteInvocationHandler : LiaraMessageHandler
    {
        public override async Task ProcessAsync(ILiaraContext context)
        {
            await context.Route.Handlers.Execute(context);

            if (context.Route.ActionReturnType == LiaraActionReturnType.Void)
            {
                ((Action<ILiaraContext>) context.Route.Action)(context);
            }
            else if (context.Route.ActionReturnType == LiaraActionReturnType.Generic)
            {
                context.Response.Content =
                    ((Func<ILiaraContext, object>) context.Route.Action)(context);
            }
            else
            {
                context.Response.Content =
                    await ((Func<ILiaraContext, Task<object>>) context.Route.Action)(context);
            }
        }
    }
}