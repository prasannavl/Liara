// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Liara.Common;

namespace Liara.Formatting
{
    public abstract class LiaraFormatter : ILiaraFormatter
    {
        public IList<MediaType> SupportedMediaTypes;
        public IList<string> SupportedUrlExtensions;

        public LiaraFormatter()
        {
            SupportedUrlExtensions = new List<string>();
            SupportedMediaTypes = new List<MediaType>();
        }

        public int Priority { get; set; }

        public virtual bool VerifyConfiguration()
        {
            if (SupportedMediaTypes.Any(supportedMediaType => supportedMediaType.ToString().Contains("*")))
            {
                throw new ArgumentException(
                    "MediaType for the formatter is invalid. A LiaraFormatter must have a specific type. " +
                    "To handle all media types, simply over-ride the CanRead/CanWrite methods to avoid the check, " +
                    "or implement ILiaraFormatter directly, which imposes no restrictions.");
            }

            if (SupportedUrlExtensions.Any(supportedUrlExtensions => supportedUrlExtensions.Contains(".")))
            {
                throw new ArgumentException("Url extensions should not include the \".\" (dot).");
            }

            return true;
        }

        public virtual MediaType GetDefaultMediaType()
        {
            return SupportedMediaTypes.FirstOrDefault();
        }

        public virtual bool CanRead(Type readAsType, ILiaraContext context)
        {
            return true;
        }

        public virtual bool CanWrite(Type inputObjectType, ILiaraContext context)
        {
            return true;
        }

        public virtual void PrepareWrite(ILiaraContext context)
        {
            var mediaType = context.Response.Format.MediaType;
            context.Response.Headers.ContentType.Value =
                mediaType + (mediaType != null ? ";" : null) +
                "charset=" +
                context.Response.Format.CharsetEncoding.WebName;
        }

        public abstract Task<object> ReadAsync(Type readAsType, Stream inputStream, ILiaraContext context);
        public abstract Task WriteAsync(object inputObject, Stream targetStream, ILiaraContext context);
    }
}