// Author: Prasanna V. Loganathar
// Project: Liara.Demos.MessageHandlers
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 11:09 AM 05-02-2014

using System.Threading.Tasks;
using Liara.MessageHandlers;

namespace Liara.Demos.MessageHandlers
{
    public class ResponseOnlyHandler : LiaraMessageHandler
    {
        public override Task ProcessAsync(ILiaraContext context)
        {
            var res = base.ProcessAsync(context);
            context.Response.WriteAsync("\r\n - Response tail added by Response only handler!");
            return res;
        }
    }
}