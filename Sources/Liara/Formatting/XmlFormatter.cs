// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Liara.Common;
using Liara.Constants;
using Liara.Helpers;

namespace Liara.Formatting
{
    public class XmlFormatter : LiaraFormatter
    {
        public XmlFormatter()
        {
            SupportedMediaTypes.Add(new MediaType(MediaTypeConstants.ApplicationXml));
            SupportedMediaTypes.Add(new MediaType(MediaTypeConstants.TextXml));
        }

        public override bool CanRead(Type readAsType, ILiaraContext context)
        {
            return readAsType != typeof (object);
        }

        public override Task<object> ReadAsync(Type readAsType, Stream inputStream, ILiaraContext context)
        {
            var ser = new XmlSerializer(readAsType);
            return Task.FromResult(ser.Deserialize(inputStream));
        }

        public override Task WriteAsync(object inputObject, Stream targetStream, ILiaraContext context)
        {
            var type = inputObject.GetType();
            if (!ReflectionHelpers.IsTypeAnonymous(type))
            {
                var xmlWriter = new XmlSerializer(type);
                xmlWriter.Serialize(targetStream, inputObject);
            }
            else
            {
                var status = context.Response.Status = LiaraHttpStatus.NotAcceptable;
                var error = new ErrorMessage
                {
                    Description = status.Description,
                    ErrorCode = status.Code,
                    Message = inputObject.ToString()
                };
                var xmlWriter = new XmlSerializer(typeof (ErrorMessage));
                xmlWriter.Serialize(targetStream, error);
            }
            return TaskHelpers.Completed();
        }
    }
}