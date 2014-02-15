// Author: Prasanna V. Loganathar
// Project: Liara.Demos.MessageHandlers
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Threading.Tasks;
using Liara.MessageHandlers;

namespace Liara.Demos.MessageHandlers
{
    public class ParallelUnorderedCompositionHandler : LiaraMessageHandler
    {
        public override async Task ProcessAsync(ILiaraContext context)
        {
            if (context.Request.Info.Path.StartsWith("/parallel"))
            {
                // Run all three handlers in parallel.
                var t1 = new ParallelHandler1().ProcessAsync(context);
                var t2 = new ParallelHandler2().ProcessAsync(context);
                var t3 = new ParallelHandler3().ProcessAsync(context);

                // Optionally, continue with other handlers in chain.
                // or add the next line to the end of the function to do that only when the parallel process is complete.
                // base.ProcessAsync(content);

                // When everything is complete, do these.
                // PS: It is async, doesn't block.

                await Task.WhenAll(new[] {t1, t2, t3});

                // All parallel stuff done. Now, do something with results. 

                // Note: It is by design that message handlers don't return values. Its just supposed to handle 
                // the messages. Not report back. So, here, the parallel handlers are writing directly to the 
                // stream. Or, the orderly result composition will take place here, which has been shown
                // on the next Handler.
            }
            else
            {
                await base.ProcessAsync(context);
            }
        }
    }

    public class ParallelHandler1 : LiaraMessageHandler
    {
        public override async Task ProcessAsync(ILiaraContext context)
        {
            // Wait 2 seconds.
            await Task.Delay(2000);
            await context.Response.WriteAsync("Hi!\r\n");
        }
    }

    public class ParallelHandler2 : LiaraMessageHandler
    {
        public override async Task ProcessAsync(ILiaraContext context)
        {
            await Task.Delay(4000);
            await context.Response.WriteAsync("I've been processed ");
        }
    }

    public class ParallelHandler3 : LiaraMessageHandler
    {
        public override async Task ProcessAsync(ILiaraContext context)
        {
            await Task.Delay(3000);
            await
                context.Response.WriteAsync(
                    String.Format("in parallel, and un-ordered. :( \r\nSlowest method time: {0} seconds.\r\n", 4));
            await context.Response.WriteAsync("I'm better suited for things where order doesn't matter. \r\n");
        }
    }
}