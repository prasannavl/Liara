// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:56 AM 15-02-2014

using System.Threading.Tasks;

namespace Liara.MessageHandlers
{
    public class TaskCompletionHandler : LiaraMessageHandler
    {
        public override Task ProcessAsync(ILiaraContext context)
        {
            return TaskHelpers.Completed();
        }
    }
}