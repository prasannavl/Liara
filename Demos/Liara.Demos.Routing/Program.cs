// Author: Prasanna V. Loganathar
// Project: Liara.Demos.Routing
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

namespace Liara.Demos.Routing
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var url = "http://localhost:12345";
            Console.WriteLine("Listening at {0}", url);
            Console.WriteLine();

            LiaraThrottleHandler.MaxConcurrentRequests = 10;
            Global.FrameworkLogger.IsEnabled = true;

            using (WebApp.Start<Startup>(url))
            {
                Process.Start(url);
                Console.ReadLine();
            }

            Console.WriteLine("Stopped");
        }
    }

    public class Startup
    {
        public ILiaraConfiguration GetLiaraConfig()
        {
            var config = new LiaraConfiguration();
            //config.Handlers.Add(new HelloHandler());
            config.Build();
            config.Services.Register(typeof (string), "The Helloku stingo!", "hellostring");
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


    //Note: Route Prefix is optional, and will automatically be chained together if nested inside modules.

    [RoutePrefix("/test")]
    public class TestModule : LiaraModule
    {
        [RoutePrefix("/modules")]
        [RoutePrefix("/mod2")]
        public class HelloModule : LiaraModule
        {
            // Path is: /test/modules/hello (or) /test/mod2/hello
            [Route("/hello"), RouteMethod("GET")]
            public void SayHelloMessage()
            {
                Response.WriteAsync("\r\nHello from the module!\r\n");
            }

            [Route("/hello2")]
            public string SayHelloUsingAutoFormat()
            {
                return "\r\nHello!! This comes auto-formatted (Serialized using the formatter).\r\n";
            }
        }
    }

    public class MyModule : TestModule
    {
        [Route("/hello3")]
        public string SayHelloUsingAutoFormat()
        {
            return "Hello3 from 3.";
        }
    }


    public class RootModule : LiaraModule
    {
        [Route("/")]
        [RouteMethod("GET, HEAD")]
        [RouteMethod("POST")]
        [Route("/new", routeName: "route2", priority: 5)]
        [RouteMethod("GET", "route2")]
        public async Task<object> SayHi()
        {
            await Task.Delay(50);
            return "\r\nJust saying hi..";
        }

        [Route("/json"), RouteMethod("POST, GET")]
        public object Json()
        {
            var c = Context.Request.Content;
            try
            {
                return new {c.Name, c.Message};
            }
            catch
            {
                return new {Error = "No name, or message given in the input!"};
            }
        }
    }
}