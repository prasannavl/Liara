// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;

namespace Liara.Formatting
{
    public interface ILiaraFormatSelector
    {
        int Priority { get; set; }
        ILiaraFormatter GetRequestFormatter(Type readAsType, ILiaraContext context);
        ILiaraFormatter GetResponseFormatter(Type inputObjectType, ILiaraContext context);
    }
}