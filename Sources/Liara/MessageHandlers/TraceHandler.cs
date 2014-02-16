// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 2:34 AM 16-02-2014

using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Liara.Common;

namespace Liara.MessageHandlers
{
    public class TraceHandler : LiaraMessageHandler
    {
        public override async Task ProcessAsync(ILiaraContext context)
        {
            var stopwatch = new Stopwatch();
            WriteEnvironment(context, false);
            stopwatch.Start();
            await base.ProcessAsync(context);
            stopwatch.Stop();
            WriteEnvironment(context, true, stopwatch.Elapsed);
        }

        public void WriteEnvironment(ILiaraContext context, bool responseOutStage, TimeSpan? timeTaken = null)
        {
            var sb = new StringBuilder();
            var separator = new string('-', 20);
            sb.AppendLine(separator);
            sb.AppendLine();
            sb.AppendLine(responseOutStage ? "Response Out - Environment Items:" : "Request In - Environment Items:");
            sb.AppendLine();

            if (responseOutStage)
            {
                sb.AppendLine();
                sb.AppendLine(string.Format("Time taken for request: {0}", timeTaken));
                sb.AppendLine();
            }

            foreach (var prop in context.Environment.GetType().GetProperties())
            {
                sb.Append(prop.Name + " : ");
                var value = prop.GetValue(context.Environment);
                if (value != null && value.GetType() == typeof (LiaraHashTable))
                {
                    sb.AppendLine();
                    sb.AppendLine();
                    var hashTable = (LiaraHashTable) value;
                    foreach (var items in hashTable)
                    {
                        sb.AppendLine(items.Key + " : " + hashTable.Get(items.Key));
                    }
                    sb.AppendLine();
                }
                else if (value != null)
                {
                    sb.AppendLine(value.ToString());
                }
                else
                {
                    sb.AppendLine();
                }
            }
            sb.AppendLine();
            sb.AppendLine(separator);
            context.Trace.WriteTo("Request Tracing", sb.ToString());
        }
    }
}