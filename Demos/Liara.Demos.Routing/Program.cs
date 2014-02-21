// Author: Prasanna V. Loganathar
// Project: Liara.Demos.Routing
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

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
            var url = "http://+:12345";

            var namedUrl = url.Replace("+", "localhost");

            Console.WriteLine("Listening at {0}", namedUrl);
            Console.WriteLine();

            Global.FrameworkLogger.IsEnabled = true;

            using (WebApp.Start<Startup>(url))
            {
                Process.Start(namedUrl);
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
            //config.Handlers.Remove(config.Handlers.FirstOrDefault(x => x.GetType() == typeof (LiaraThrottleHandler)));

            //var throttleHandler = new LiaraThrottleHandler {MaxConcurrentRequests = 10};
            //config.Handlers.Insert(0, throttleHandler);

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
        public object SayHi()
        {
            return "\r\nJust saying hi..";
        }

        [Route("/post"), RouteMethod("POST")]
        public object Post1()
        {
            var c = Request.Content;

            try
            {
                return new {Name = c.Name, Message = c.Message};
            }
            catch (Exception ex)
            {
                return new {Error = "No name, or message given in the input!", Detail = ex.Message};
            }
        }

        [Route("/postparams"), RouteMethod("POST")]
        public object Post2()
        {
            try
            {
                return new { Name = Request.Parameters["Name"], Message = Request.Parameters["Message"] };
            }
            catch (Exception ex)
            {
                return new { Error = "No name, or message given in the input!", Detail = ex.Message };
            }
        }

        class PostDto
        {
            public string Name { get; set; }
            public  string Message { get; set; }
        }

        [RequestModel(typeof(PostDto))]
        [Route("/postdto"), RouteMethod("POST")]
        public object PostDtoAction()
        {
            PostDto c = Request.Content;

            try
            {
                return new { Name = c.Name, Message = c.Message };
            }
            catch (Exception ex)
            {
                return new { Error = "No name, or message given in the input!", Detail = ex.Message };
            }
        }


        [RequestModel(typeof(PostDto))]
        [Route("/postdto2"), RouteMethod("POST")]
        public object PostDto2Action()
        {
            try
            {
                return new { Name = Request.Parameters["Name"], Message = Request.Parameters["Message"] };
            }
            catch (Exception ex)
            {
                return new { Error = "No name, or message given in the input!", Detail = ex.Message };
            }
        }
    }
}