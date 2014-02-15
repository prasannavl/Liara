// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Collections.Generic;
using System.Reflection;
using Liara.MessageHandlers;

namespace Liara.Routing
{
    public class AggregatedRoute
    {
        public AggregatedRoute()
        {
            RequestMethods = new List<string>();
            Handlers = new LiaraMessageHandlerCollection();
            Conditions = new List<Func<LiaraContext, bool>>();
            RouteItems = new Dictionary<string, object>();
        }

        public string Path { get; set; }
        public Type RequestDtoType { get; set; }
        public IList<string> RequestMethods { get; set; }
        public MethodInfo ActionMethodInfo { get; set; }
        public LiaraMessageHandlerCollection Handlers { get; set; }
        public IList<Func<LiaraContext, bool>> Conditions { get; set; }
        public IDictionary<string, object> RouteItems { get; set; }
        public int Priority { get; set; }
    }
}