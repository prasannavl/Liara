// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Liara.Common;

namespace Liara.Formatting
{
    public abstract class LiaraMediaTypeBasedFormatter : ILiaraFormatter
    {
        public IList<MediaType> SupportedMediaTypes;

        public LiaraMediaTypeBasedFormatter()
        {
            SupportedMediaTypes = new List<MediaType>();
        }

        public int Priority { get; set; }

        public virtual bool VerifyConfiguration()
        {
            if (SupportedMediaTypes.Any(supportedMediaType => supportedMediaType.ToString().Contains("*")))
            {
                throw new ArgumentException(
                    "MediaType for the formatter is invalid. A specific media type must be given. " +
                    "To handle all media types, simply over-ride the CanRead/CanWrite methods to avoid the check.");
            }

            return true;
        }

        public virtual bool CanRead(Type readAsType, ILiaraContext context)
        {
            var mediaType = context.Request.Format.MediaType;
            return mediaType != null && SupportedMediaTypes.Any(sm => sm.Equals(mediaType));
        }

        public virtual bool CanWrite(Type inputObjectType, ILiaraContext context)
        {
            var accepted = false;
            foreach (var acceptedMediaType in context.Response.Format.AcceptedMediaTypes)
            {
                var mediaType = (MediaType) acceptedMediaType;

                foreach (var supportedMediaType in SupportedMediaTypes)
                {
                    var isAcceptable = supportedMediaType.IsCompatible(mediaType);
                    if (isAcceptable)
                    {
                        context.Response.Format.MediaType = supportedMediaType;
                        accepted = true;
                        break;
                    }
                }
                if (accepted) break;
            }

            return accepted;
        }

        public virtual void PrepareWrite(ILiaraContext context)
        {
            context.Response.Headers.ContentType.Value =
                context.Response.Format.MediaType +
                ";charset=" +
                context.Response.Format.CharsetEncoding.WebName;
        }

        public abstract Task<object> ReadAsync(Type readAsType, Stream inputStream, ILiaraContext context);
        public abstract Task WriteAsync(object inputObject, Stream targetStream, ILiaraContext context);
    }
}