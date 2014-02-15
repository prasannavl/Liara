// Author: Prasanna V. Loganathar
// Project: Liara.Demos.Formatting
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 11:22 AM 05-02-2014

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Liara.MessageHandlers;
using Microsoft.Owin.Hosting;
using Owin;

namespace Liara.Demos.Formatting
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
        public override async Task ProcessAsync(ILiaraContext context)
        {
            // Anything put into content will be serialized into the stream, using the Formatter, 
            // which is in-turn automatically selected using the FormatSelector.

            // Uses the default TextFormatter here.


            //await Task.Delay(10000);
            var t = new TestType {Message = "Hello!", RequestPath = context.Request.Info.Uri.ToString()};
            context.Response.Content = t;
            await base.ProcessAsync(context);
        }

        public class TestType
        {
            public string Message { get; set; }
            public string RequestPath { get; set; }
        }
    }

    public class HelloModule : LiaraModule
    {
        [Route("/")]
        public async void Test()
        {
        }
    }
}