// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Linq;

namespace Liara.Formatting
{
    public class LiaraFormatSelector : ILiaraFormatSelector
    {
        public virtual int Priority { get; set; }

        public virtual ILiaraFormatter GetRequestFormatter(Type readAsType, ILiaraContext context)
        {
            return
                context.Engine.Configuration.Formatters.FirstOrDefault(
                    liaraFormatter => liaraFormatter.CanRead(readAsType, context));
        }

        public virtual ILiaraFormatter GetResponseFormatter(Type inputObjectType, ILiaraContext context)
        {
            return
                context.Engine.Configuration.Formatters.FirstOrDefault(
                    liaraFormatter => liaraFormatter.CanWrite(inputObjectType, context));
        }
    }
}