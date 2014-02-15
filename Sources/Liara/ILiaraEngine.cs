// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System.Threading.Tasks;

namespace Liara
{
    public interface ILiaraEngine
    {
        ILiaraConfiguration Configuration { get; set; }
        Task ProcessRequestAsync(ILiaraServerEnvironment env);
    }
}