// Author: Prasanna V. Loganathar
// Project: Liara.Demos.Hosting.Owin
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Liara.MessageHandlers;
using Microsoft.Owin.Hosting;
using Owin;

namespace Liara.Demos.Hosting.Owin
{
    public class Program
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

    public class Startup
    {
        public ILiaraConfiguration GetLiaraConfig()
        {
            var config = new LiaraConfiguration();
            config.Handlers.Add(new HelloHandler());
            return config;
        }

        public void Configuration(IAppBuilder app)
        {
            app.UseErrorPage();
            app.UseLiara(GetLiaraConfig());
        }
    }

    public class HelloHandler : LiaraMessageHandler
    {
        public override Task ProcessAsync(ILiaraContext context)
        {
            context.Response.WriteAsync("Hello!\r\n");
            context.Response.WriteAsync(context.Request.Info.Uri.ToString());

            return base.ProcessAsync(context);
        }
    }

    public class HelloModule : LiaraModule
    {
        [Route("/")]
        public void RootFunction()
        {
        }
    }
}