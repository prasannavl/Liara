// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Liara.Common;
using Liara.Formatting;
using Liara.Helpers;

namespace Liara.RequestProcessing
{
    public class LiaraRequestFormatProvider : ILiaraRequestFormatProvider
    {
        private readonly ILiaraContext context;
        private Encoding charsetEncoding;
        private ILiaraFormatter formatter;
        private bool isHeaderRead;
        private MediaType mediaType;

        public LiaraRequestFormatProvider(ILiaraContext context)
        {
            this.context = context;
        }

        public MediaType MediaType
        {
            get
            {
                if (!isHeaderRead)
                {
                    Detect();
                }
                return mediaType;
            }
            set { mediaType = value; }
        }

        public Encoding CharsetEncoding
        {
            get
            {
                if (!isHeaderRead)
                {
                    Detect();
                }
                return charsetEncoding;
            }
            set { charsetEncoding = value; }
        }

        public bool IsSelected { get; set; }
        public bool IsProcessed { get; set; }

        public ILiaraFormatter Formatter
        {
            get
            {
                if (!IsSelected)
                {
                    SelectFormatter();
                }
                return formatter;
            }
            set
            {
                formatter = value;
                IsSelected = true;
            }
        }

        public Task<object> ProcessAsync(Type readAsType, Stream stream)
        {
            if (Formatter == null)
            {
                context.Response.Status = LiaraHttpStatus.NotAcceptable;
                return TaskHelpers.NullResult();
            }

            IsProcessed = true;
            try
            {
                return Formatter.ReadAsync(readAsType, stream, context);
            }
            catch (Exception ex)
            {
                context.Response.Status = LiaraHttpStatus.BadRequest;
                context.Log.WriteExceptionToAsync("Bad Requests and Format Error", ex);
                return TaskHelpers.NullResult();
            }
        }

        public void SelectFormatter()
        {
            formatter =
                context.Engine.Configuration.FormatSelector.GetRequestFormatter(
                    context.Route.RequestDtoType ?? typeof (object), context);
            IsSelected = true;
        }

        public void Detect()
        {
            if (context.Request.Headers.ContentType.HasValue)
            {
                HeaderHelpers.GetContentMediaTypeAndCharset(context.Request.Headers.ContentType.Value, out mediaType,
                    out charsetEncoding);
            }
            if (mediaType == null)
            {
                mediaType = MediaType.None;
            }
            if (charsetEncoding == null)
            {
                charsetEncoding = Encoding.UTF8;
            }
            isHeaderRead = true;
        }
    }
}