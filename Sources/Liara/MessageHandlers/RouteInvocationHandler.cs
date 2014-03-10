// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:20 AM 10-03-2014

using System.Threading.Tasks;

namespace Liara.MessageHandlers
{
    public class RouteInvocationHandler : LiaraMessageHandler
    {
        public override Task ProcessAsync(ILiaraContext context)
        {
            return context.Route.Handlers.Execute(context);
        }
    }
}