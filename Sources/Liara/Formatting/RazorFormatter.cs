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
    // TODO
    public class RazorFormatter : ILiaraFormatter
    {
        public int Priority { get; set; }

        public bool VerifyConfiguration()
        {
            return false;
        }

        public bool CanRead(Type readAsType, ILiaraContext context)
        {
            return false;
        }

        public bool CanWrite(Type inputObjectType, ILiaraContext context)
        {
            return false;
        }

        public void PrepareWrite(ILiaraContext context)
        {
        }

        public Task<object> ReadAsync(Type readAsType, Stream inputStream, ILiaraContext context)
        {
            return null;
        }

        public Task WriteAsync(object inputObject, Stream targetStream, ILiaraContext context)
        {
            return null;
        }
    }
}