// Author: Prasanna V. Loganathar
// Project: Liara.Hosting.Owin
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Liara.Hosting.Owin;

namespace Liara
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class LiaraMiddleware
    {
        private readonly LiaraEngine engine;
        private readonly AppFunc next;

        /// <summary>
        ///     Creates a new Liara Engine for applications.
        /// </summary>
        /// <param name="next" />
        /// <param name="configuration"></param>
        public LiaraMiddleware(AppFunc next, LiaraConfiguration configuration = null)
        {
            if (configuration == null)
                configuration = new LiaraConfiguration();

            engine = new LiaraEngine(configuration);
            this.next = next;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            var liaraEnv = new OwinServerEnvironment(env, engine);
            await engine.ProcessRequestAsync(liaraEnv);
        }
    }
}