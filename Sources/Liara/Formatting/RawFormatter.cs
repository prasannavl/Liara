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
    public class RawFormatter : ILiaraFormatter
    {
        private int priority = -2;

        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        public bool VerifyConfiguration()
        {
            return true;
        }

        public bool CanRead(Type readAsType, ILiaraContext context)
        {
            return false;
        }

        public bool CanWrite(Type inputObjectType, ILiaraContext context)
        {
            return true;
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
            var bytes = context.Response.Format.CharsetEncoding.GetBytes(inputObject.ToString());
            return targetStream.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}