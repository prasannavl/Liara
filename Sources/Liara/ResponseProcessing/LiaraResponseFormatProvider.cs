// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liara.Common;
using Liara.Formatting;
using Liara.Helpers;

namespace Liara.ResponseProcessing
{
    public class LiaraResponseFormatProvider : ILiaraResponseFormatProvider
    {
        private readonly ILiaraContext context;

        private IEnumerable<Encoding> acceptedCharsetEncoding;
        private IEnumerable<CultureInfo> acceptedCultures;
        private IEnumerable<QValuedMediaType> acceptedMediaTypes;
        private Encoding charsetEncoding;
        private CultureInfo culture;
        private ILiaraFormatter formatter;
        private MediaType mediaType;

        public LiaraResponseFormatProvider(ILiaraContext context)
        {
            this.context = context;
        }

        public MediaType MediaType
        {
            get
            {
                if (mediaType == null) SelectFormatter();
                return mediaType;
            }
            set { mediaType = value; }
        }

        public Encoding CharsetEncoding
        {
            get
            {
                if (charsetEncoding == null) SelectCharsetEncoding();
                return charsetEncoding;
            }
            set { charsetEncoding = value; }
        }

        public CultureInfo Culture
        {
            get
            {
                if (culture == null) SelectCulture();
                return culture;
            }
            set { culture = value; }
        }

        public string View { get; set; }

        public IEnumerable<QValuedMediaType> AcceptedMediaTypes
        {
            get
            {
                return acceptedMediaTypes ?? (acceptedMediaTypes = HeaderHelpers.GetPrioritizedQValuedMediaTypes(
                    HeaderHelpers.GetQValuedMediaTypeHeaders(context.Request.Headers.Accept.Value)));
            }

            set { acceptedMediaTypes = value; }
        }

        public IEnumerable<Encoding> AcceptedCharsetEncoding
        {
            get
            {
                return acceptedCharsetEncoding ??
                       (acceptedCharsetEncoding =
                           HeaderHelpers.GetCharsetAcceptedEncodings(context.Request.Headers.AcceptCharset.Value));
            }
            set { acceptedCharsetEncoding = value; }
        }

        public IEnumerable<CultureInfo> AcceptedCultures
        {
            get
            {
                return acceptedCultures ??
                       (acceptedCultures =
                           HeaderHelpers.GetAcceptedCultures(context.Request.Headers.AcceptLanguage.Value));
            }
            set { acceptedCultures = value; }
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

        public Task ProcessAsync(object inputObject, Stream targetStream)
        {
            if (Formatter == null)
                throw new InvalidOperationException("No response formatter available.");

            IsProcessed = true;
            return Formatter.WriteAsync(inputObject, targetStream, context);
        }

        public void SelectAll()
        {
            SelectFormatter();
            SelectCharsetEncoding();
            SelectCulture();
        }

        public void SelectFormatter()
        {
            Type type = null;
            if (context.Response.Content != null)
                type = context.Response.Content.GetType();
            Formatter = context.Engine.Configuration.FormatSelector.GetResponseFormatter(type, context);
        }

        public void SelectCharsetEncoding()
        {
            charsetEncoding = AcceptedCharsetEncoding.FirstOrDefault() ?? Encoding.UTF8;
        }

        public void SelectCulture()
        {
            culture = AcceptedCultures.FirstOrDefault() ?? CultureInfo.CurrentCulture;
        }
    }
}