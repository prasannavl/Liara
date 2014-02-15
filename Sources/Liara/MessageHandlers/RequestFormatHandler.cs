// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:56 AM 15-02-2014

using System.IO;
using System.Threading.Tasks;

namespace Liara.MessageHandlers
{
    public class RequestFormatHandler : LiaraMessageHandler
    {
        public override async Task ProcessAsync(ILiaraContext context)
        {
            if (context.Environment.RequestBody != null)
            {
                var stream = (Stream) context.Environment.RequestBody;
                if (context.Request.Format.Formatter != null)
                {
                    context.Request.Content = await context.Request.Format.ProcessAsync(
                        context.Route.RequestDtoType ?? typeof (object),
                        stream);
                    if (context.Response.Status == LiaraHttpStatus.BadRequest ||
                        context.Response.Status == LiaraHttpStatus.NotAcceptable)
                    {
                        return;
                    }
                }
            }

            await base.ProcessAsync(context);
        }
    }
}