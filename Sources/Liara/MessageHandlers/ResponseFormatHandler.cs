// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System.Threading.Tasks;

namespace Liara.MessageHandlers
{
    public class ResponseFormatHandler : LiaraMessageHandler
    {
        public override async Task ProcessAsync(ILiaraContext context)
        {
            await base.ProcessAsync(context);

            // Use the response synchronization before writing to the stream.
            // Note: Make sure response synchronize is called at the end, in a case where this message handler is removed
            // from the chain, or replaced.

            context.Response.Synchronize();

            if (context.Response.Content != null && context.Response.Format.Formatter != null)
            {
                await context.Response.Format.ProcessAsync(
                    context.Response.Content,
                    context.Environment.ResponseBody);
            }
        }
    }
}