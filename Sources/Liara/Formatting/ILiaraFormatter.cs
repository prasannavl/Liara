﻿// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

using System;
using System.IO;
using System.Threading.Tasks;
using Liara.Common;

namespace Liara.Formatting
{
    public interface ILiaraFormatter : ILiaraPrioritizedService
    {
        bool VerifyConfiguration();
        bool CanRead(Type readAsType, ILiaraContext context);
        bool CanWrite(Type inputObjectType, ILiaraContext context);
        MediaType GetDefaultMediaType();
        void PrepareWrite(ILiaraContext context);
        Task<object> ReadAsync(Type readAsType, Stream inputStream, ILiaraContext context);
        Task WriteAsync(object inputObject, Stream targetStream, ILiaraContext context);
    }
}