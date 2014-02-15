// Author: Prasanna V. Loganathar
// Project: Liara.Hosting.Owin
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using Liara;

namespace Owin
{
    public static class LiaraExtensions
    {
        public static IAppBuilder UseLiara(this IAppBuilder builder, ILiaraConfiguration configuration = null)
        {
            return builder.Use(typeof (LiaraMiddleware), configuration);
        }
    }
}