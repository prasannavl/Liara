// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System.Threading.Tasks;

namespace Liara
{
    public class LiaraEngine : ILiaraEngine
    {
        public LiaraEngine(ILiaraConfiguration config)
        {
            config.Build();
            Configuration = config;
        }

        public ILiaraConfiguration Configuration { get; set; }

        public Task ProcessRequestAsync(ILiaraServerEnvironment environment)
        {
            using (var context = new LiaraContext(this, environment))
            {
                return Configuration.Handlers.Execute(context);
            }
        }
    }
}