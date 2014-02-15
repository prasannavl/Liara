// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System.Threading.Tasks;

namespace Liara.MessageHandlers
{
    public class HttpStatusHandler : LiaraMessageHandler
    {
        public override async Task ProcessAsync(ILiaraContext context)
        {
            await base.ProcessAsync(context);
            context.Engine.Configuration.StatusHandler.HandleStatus(context);
        }
    }
}