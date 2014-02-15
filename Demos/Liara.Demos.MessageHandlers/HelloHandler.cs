// Author: Prasanna V. Loganathar
// Project: Liara.Demos.MessageHandlers
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System.Threading.Tasks;
using Liara.MessageHandlers;

namespace Liara.Demos.MessageHandlers
{
    public class HelloHandler : LiaraMessageHandler
    {
        public override Task ProcessAsync(ILiaraContext context)
        {
            context.Response.WriteAsync("Hello!\r\n");
            context.Response.WriteAsync(context.Request.Info.Uri.ToString());

            return base.ProcessAsync(context);
        }
    }
}