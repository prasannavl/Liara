// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.IO;
using System.Threading.Tasks;

namespace Liara.Formatting
{
    public interface ILiaraFormatProvider
    {
        bool IsProcessed { get; set; }
        bool IsSelected { get; set; }
        ILiaraFormatter Formatter { get; set; }
    }

    public interface ILiaraRequestFormatProvider : ILiaraFormatProvider
    {
        Task<object> ProcessAsync(Type readAsType, Stream inputStream);
    }

    public interface ILiaraResponseFormatProvider : ILiaraFormatProvider
    {
        Task ProcessAsync(object inputObject, Stream targetStream);
    }
}