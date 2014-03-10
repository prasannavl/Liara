// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

using System;
using System.Linq;
using Liara.Common;
using Liara.Constants;

namespace Liara.Formatting
{
    public class LiaraFormatSelector : ILiaraFormatSelector
    {
        private int priority = LiaraServiceConstants.PriorityLow;

        public virtual int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        public virtual ILiaraFormatter GetRequestFormatter(Type readAsType, ILiaraContext context)
        {
            ILiaraFormatter[] formatterArray;

            if (context.Engine.Configuration.Formatters.MediaMap.TryGetValue(
                context.Request.Format.MediaType.ToString().ToLower(),
                out formatterArray))
            {
                foreach (var liaraFormatter in formatterArray)
                {
                    if (liaraFormatter.CanRead(readAsType, context))
                        return liaraFormatter;
                }
            }

            return
                context.Engine.Configuration.Formatters.FirstOrDefault(
                    formatter => formatter.CanRead(readAsType, context));
        }

        public virtual ILiaraFormatter GetResponseFormatter(Type inputObjectType, ILiaraContext context)
        {
            ILiaraFormatter[] formatterArray;

            if (context.Route != null && context.Route.PathExtension != null)
            {
                if (context.Engine.Configuration.Formatters.UrlMap.TryGetValue(
                    context.Route.PathExtension.ToLower(),
                    out formatterArray))
                {
                    foreach (var liaraFormatter in formatterArray)
                    {
                        if (liaraFormatter.CanWrite(inputObjectType, context))
                        {
                            context.Response.Format.MediaType = liaraFormatter.GetDefaultMediaType();
                            return liaraFormatter;
                        }
                    }
                }
            }

            foreach (var acceptedMediaType in context.Response.Format.AcceptedMediaTypes)
            {
                if (context.Engine.Configuration.Formatters.MediaMap.TryGetValue(
                    MediaType.FromDerivedMediaType(acceptedMediaType).ToString().ToLower(), out formatterArray))
                {
                    foreach (var liaraFormatter in formatterArray)
                    {
                        if (liaraFormatter.CanWrite(inputObjectType, context))
                        {
                            context.Response.Format.MediaType = acceptedMediaType;
                            return liaraFormatter;
                        }
                    }
                }
            }

            return
                context.Engine.Configuration.Formatters.FirstOrDefault(
                    formatter => formatter.CanWrite(inputObjectType, context));
        }
    }
}