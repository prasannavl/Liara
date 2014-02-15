// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 2:34 AM 16-02-2014

using System.Text;
using System.Threading.Tasks;
using Liara.Common;

namespace Liara.MessageHandlers
{
    public class TraceHandler : LiaraMessageHandler
    {
        public override async Task ProcessAsync(ILiaraContext context)
        {
            WriteEnvironment(context, true);
            await base.ProcessAsync(context);
            WriteEnvironment(context, false);
        }

        public void WriteEnvironment(ILiaraContext context, bool requestIn)
        {
            var sb = new StringBuilder();
            sb.AppendLine("-----------------");
            sb.AppendLine();
            sb.AppendLine(requestIn ? "Request-In - Environment Items:" : "Response-Out - Environment Items:");
            sb.AppendLine();
            sb.AppendLine();
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
            sb.AppendLine("-----------------");
            context.Trace.WriteTo("Request Tracing", sb.ToString());
        }
    }
}