// Author: Prasanna V. Loganathar
// Project: Liara.Demos.MessageHandlers
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Diagnostics;
using Microsoft.Owin.Hosting;
using Owin;

namespace Liara.Demos.MessageHandlers
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var url = "http://localhost:12345";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Listening at {0}", url);
                Process.Start(url);
                Console.ReadLine();
            }
        }
    }

    public class HelloModule : LiaraModule
    {
        [Route("/")]
        [Route("/parallel")]
        [Route("/parallel/ordered")]
        public void RootFunction()
        {
        }
    }


    /// <summary>
    ///     Direct Handler routes for the demo:
    ///     Route 1: /
    ///     Route 2: /parallel
    ///     Route 3: /parallel/ordered
    /// </summary>
    public class Startup
    {
        public ILiaraConfiguration GetLiaraConfig()
        {
            var config = new LiaraConfiguration();
            config.Handlers.Add(new ParallelOrderedCompositionHandler());
            config.Handlers.Add(new ParallelUnorderedCompositionHandler());
            config.Handlers.Add(new HelloHandler());
            config.Handlers.Add(new ResponseOnlyHandler());
            return config;
        }

        public void Configuration(IAppBuilder app)
        {
            app.UseErrorPage();
            app.UseLiara(GetLiaraConfig());
        }
    }
}