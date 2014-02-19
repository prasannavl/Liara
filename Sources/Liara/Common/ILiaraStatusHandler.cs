// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

namespace Liara.Common
{
    public interface ILiaraStatusHandler : ILiaraPrioritizedService
    {
        bool HandleStatus(ILiaraContext context);
    }
}