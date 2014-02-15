// Author: Prasanna V. Loganathar
// Project: Liara.Demos.MessageHandlers
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 11:09 AM 05-02-2014

using System.Linq;
using System.Threading.Tasks;
using Liara.MessageHandlers;

namespace Liara.Demos.MessageHandlers
{
    public class ParallelOrderedCompositionHandler : LiaraMessageHandler
    {
        public override async Task ProcessAsync(ILiaraContext context)
        {
            if (context.Request.Info.Path.StartsWith("/parallel/ordered"))
            {
                var t1 = Task.Run(() => ParallelLambdas.Process1());
                var t2 = Task.Run(() => ParallelLambdas.Process2());
                var t3 = Task.Run(() => ParallelLambdas.Process3());
                var t4 = Task.Run(() => ParallelLambdas.Process4());

                await Task.WhenAll(new[] {t1, t2, t3, t4});


                new[] {t1.Result, t2.Result, t3.Result, t4.Result}.ToList()
                    .ForEach(async text => await context.Response.WriteAsync(text));
            }
            else
            {
                await base.ProcessAsync(context);
            }
        }
    }

    public static class ParallelLambdas
    {
        public static async Task<string> Process1()
        {
            // Wait 5 seconds.
            await Task.Delay(5000);
            return "Hi!\r\n";
        }

        public static async Task<string> Process2()
        {
            // Wait 2 seconds.
            await Task.Delay(2000);
            return "I ran in ";
        }

        public static async Task<string> Process3()
        {
            // Wait 1 seconds.
            await Task.Delay(1000);
            return "parallel, and ";
        }

        public static async Task<string> Process4()
        {
            // Wait 3 seconds.
            await Task.Delay(3000);
            return "came in the correct order!.\r\nSlowest method time: 5 seconds.";
        }
    }
}