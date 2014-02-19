// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

using System;
using System.Collections.Generic;
using System.Reflection;
using Liara.MessageHandlers;

namespace Liara.Routing
{
    public class Route
    {
        public Route()
        {
            Id = Guid.NewGuid();
            Handlers = new LiaraMessageHandlerCollection();
            Parameters = new Dictionary<string, string>();
        }

        public Guid Id { get; set; }
        public string Path { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public LiaraActionReturnType ActionReturnType { get; set; }
        public Type RequestDtoType { get; set; }
        public LiaraMessageHandlerCollection Handlers { get; set; }
        public object Action { get; set; }
        public int Priority { get; set; }
        public Func<ILiaraContext, bool>[] Conditions { get; set; }
        public IDictionary<string, string> Parameters { get; set; }
        public string PathExtension { get; set; }
    }

    public enum LiaraActionReturnType
    {
        Void,
        Task,
        Generic
    }
}