// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Threading.Tasks;

namespace Liara.MessageHandlers
{
    public class ErrorHandler : LiaraMessageHandler
    {
        public override async Task ProcessAsync(ILiaraContext context)
        {
            try
            {
                await base.ProcessAsync(context);
            }
            catch (Exception exception)
            {
                context.Response.Status = LiaraHttpStatus.InternalServerError;
                // Log asynchronously, and don't wait for completion.
                var task = context.Log.WriteExceptionAsync(exception);
            }
        }
    }
}