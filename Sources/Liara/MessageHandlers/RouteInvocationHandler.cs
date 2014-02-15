// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:56 AM 15-02-2014

using System;
using System.Threading.Tasks;

namespace Liara.MessageHandlers
{
    /// <summary>
    ///     Route invocation handler. Usually the last in the handler chain.
    ///     Calls the route action, typically inside the LiaraModule.
    /// </summary>
    public class RouteInvocationHandler : LiaraMessageHandler
    {
        public override Task ProcessAsync(ILiaraContext context)
        {
            if (context.Route.MethodInfo.ReturnType == typeof (void))
            {
                ((Action<ILiaraContext>) context.Route.Action)(context);
            }
            else
            {
                context.Response.Content =
                    ((Func<ILiaraContext, object>) context.Route.Action)(context);
            }

            return TaskHelpers.Completed();
        }
    }
}