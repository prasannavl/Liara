// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System.Threading.Tasks;

namespace Liara
{
    public interface ILiaraEngine
    {
        ILiaraConfiguration Configuration { get; set; }
        Task ProcessRequestAsync(ILiaraServerEnvironment env);
    }
}