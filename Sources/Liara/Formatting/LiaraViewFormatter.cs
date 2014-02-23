// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 7:55 PM 17-02-2014

using System;
using System.IO;
using System.Threading.Tasks;
using Liara.Common;
using Liara.Constants;

namespace Liara.Formatting
{
    public abstract class LiaraViewFormatter : LiaraFormatter
    {
        private ILiaraViewProvider viewProvider;

        public LiaraViewFormatter()
        {
            Priority = LiaraServiceConstants.PriorityDefault;

            SupportedMediaTypes.Add(new MediaType(Constants.MediaTypeConstants.TextHtml));
            SupportedMediaTypes.Add(new MediaType(Constants.MediaTypeConstants.ApplicationXhtmlXml));
        }

        public ILiaraViewProvider ViewProvider
        {
            get { return viewProvider ?? (viewProvider = new LiaraViewProvider()); }
            set { viewProvider = value; }
        }

        public override bool CanRead(Type readAsType, ILiaraContext context)
        {
            return false;
        }

        public override Task<object> ReadAsync(Type readAsType, Stream inputStream, ILiaraContext context)
        {
            return null;
        }

        public abstract Task<string> RenderView(ILiaraViewTemplate viewTemplate, object model, ILiaraContext context);

        public virtual ILiaraViewTemplate ResolveInternalView(ILiaraContext context)
        {
            if (context.Response.Status.Code != LiaraHttpStatus.OK.Code)
            {
                var viewName = "_liara_" + context.Response.Status.Code;
                var templateString = ViewProvider.GetInternalView(context);
                return new LiaraViewTemplate(viewName, templateString, true);
            }
            return null;
        }

        public virtual ILiaraViewTemplate ResolveView(ILiaraContext context)
        {
            var templateString = ViewProvider.GetView(context);
            if (templateString == null)
            {
                context.Response.Status = LiaraHttpStatus.NoViewAssociated;
                return ResolveInternalView(context);
            }
            var viewName = context.Response.Format.View;
            if (viewName == null)
            {
                if (context.Route != null)
                {
                    viewName = context.Route.Id.ToString();
                }
            }
            return new LiaraViewTemplate(viewName, templateString);
        }

        public override async Task WriteAsync(object inputObject, Stream targetStream, ILiaraContext context)
        {
            var template = ResolveInternalView(context) ?? ResolveView(context);
            if (template.IsInternalTemplate)
            {
                if (inputObject.GetType() != typeof (ErrorMessage))
                {
                    inputObject = new ErrorMessage
                    {
                        ErrorCode = context.Response.Status.Code,
                        Description = context.Response.Status.Description,
                        Message = inputObject
                    };
                }
            }
            var viewOutput = await RenderView(template, inputObject, context);
            var buf = context.Response.Format.CharsetEncoding.GetBytes(viewOutput);
            await targetStream.WriteAsync(buf, 0, buf.Length);
        }
    }
}