// Author: Prasanna V. Loganathar
// Project: Liara.Hosting.Owin
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using Liara.ResponseProcessing;

namespace Liara.Hosting.Owin
{
    public class LiaraOwinResponseSynchronizer : ILiaraResponseSynchronizer
    {
        public int Priority { get; set; }

        public void Synchronize(ILiaraContext context)
        {
            context.Response.Cookies.FlushToHeaders();
        }
    }
}