// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 1:16 PM 15-02-2014

namespace Liara.Services
{
    public interface ILiaraServiceDiscovery
    {
        int Priority { get; set; }
        void Discover();
    }
}