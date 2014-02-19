// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 1:54 PM 18-02-2014

using System.Collections.Generic;

namespace Liara.Common
{
    public interface ILiaraPrioritizedService
    {
        int Priority { get; set; }
    }

    public interface ILiaraPrioritizedServiceCollection<T> : IList<T> where T : ILiaraPrioritizedService
    {

    }
}