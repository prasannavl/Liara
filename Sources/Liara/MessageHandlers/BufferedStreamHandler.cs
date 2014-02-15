// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System.IO;
using System.Threading.Tasks;
using Liara.Common;

namespace Liara.MessageHandlers
{
    public class BufferedStreamHandler : LiaraMessageHandler
    {
        public override async Task ProcessAsync(ILiaraContext context)
        {
            var requestStream = (LiaraStream) context.Request.Content;
            if (requestStream != null && requestStream.IsBuffered)
            {
                await requestStream.BufferFillTask;
            }

            await base.ProcessAsync(context);

            var responseStream = context.Environment.ResponseBody;
            if (responseStream.IsBuffered)
            {
                responseStream.Seek(0, SeekOrigin.Begin);
                if (!responseStream.IsBufferFilled)
                {
                    await responseStream.BufferFillTask;
                }
                await responseStream.CopyToAsync(context.Environment.ResponseBody);
            }
        }
    }
}