// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

using System;
using System.Deployment.Internal;
using System.Threading;
using System.Threading.Tasks;
using Liara.Constants;
using Liara.Routing;

namespace Liara.MessageHandlers
{

    public interface ILiaraActionInvoker
    {
        Task ProcessAsync(ILiaraContext context);
    }

    /// <summary>
    ///     Route invocation handler. Usually the last in the handler chain.
    ///     Calls the route specific handlers, and the route action, typically inside the LiaraModule.
    /// </summary>
    public class ActionInvocationHandler : LiaraMessageHandler, ILiaraActionInvoker
    {
        public override async Task ProcessAsync(ILiaraContext context)
        {
            try
            {
                switch (context.Route.ActionReturnType)
                {
                    case LiaraActionReturnType.Void:
                        ((Action<ILiaraContext>) context.Route.Action)(context);
                        break;
                    case LiaraActionReturnType.Generic:
                        context.Response.Content =
                            ((Func<ILiaraContext, object>) context.Route.Action)(context);
                        break;
                    case LiaraActionReturnType.Task:
                        context.Response.Content =
                            await ((Func<ILiaraContext, Task<object>>) context.Route.Action)(context);
                        break;
                }
            }
            catch (Exception ex)
            {
                context.Response.Status = LiaraHttpStatus.InternalServerError;
                context.Response.Content = ex;
            }
        }
    }
}