// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.IO;
using System.Threading.Tasks;
using Liara.Common;
using Liara.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Liara.Formatting
{
    public class JsonFormatter : LiaraFormatter
    {
        private readonly JsonSerializerSettings settings;

        public JsonFormatter()
        {
            SupportedUrlExtensions.Add("json");

            SupportedMediaTypes.Add(new MediaType(MediaTypeConstants.ApplicationJson));
            SupportedMediaTypes.Add(new MediaType(MediaTypeConstants.TextJson));

            settings = new JsonSerializerSettings();
        }

        public override bool CanWrite(Type inputObjectType, ILiaraContext context)
        {
            return true;
        }

        public override Task<object> ReadAsync(Type readAsType, Stream inputStream, ILiaraContext context)
        {
            var output = JObject.Parse(
                new StreamReader(inputStream,
                    context.Request.Format.CharsetEncoding,
                    false,
                    4096,
                    true).ReadToEnd());

            if (readAsType == typeof (object))
            {
                object res = output;
                return Task.FromResult(res);
            }
            return Task.FromResult(output.ToObject(readAsType));
        }

        public override Task WriteAsync(object inputObject, Stream targetStream, ILiaraContext context)
        {
            // TODO: Optimize memory - Streamed conversion.
            var objType = inputObject.GetType();
            var value = JsonConvert.SerializeObject(inputObject, objType, settings);
            var bytes = context.Response.Format.CharsetEncoding.GetBytes(value);
            return targetStream.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}