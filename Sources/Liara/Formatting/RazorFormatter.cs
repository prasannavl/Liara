// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

using System.Threading.Tasks;
using Liara.Common;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace Liara.Formatting
{
    public class RazorViewFormatter : LiaraViewFormatter
    {
        private readonly TemplateService service;

        public RazorViewFormatter()
        {
            ViewProvider = new RazorViewProvider();
            var config = new TemplateServiceConfiguration {BaseTemplateType = typeof (LiaraRazorTemplateBase<>)};
#if DEBUG
            config.Debug = true;            
#endif
            config.Namespaces.Add("System");
            config.Namespaces.Add("System.IO");
            config.Namespaces.Add("System.Linq");
            config.Namespaces.Add("Liara");
            config.Namespaces.Add("Liara.Common");
            config.Namespaces.Add("Liara.Extensions");
            config.Namespaces.Add("Liara.Helpers");
            config.Namespaces.Add("Liara.ResponseProcessing");
            config.Namespaces.Add("Liara.RequestProcessing");

            service = new TemplateService(config);
        }

        public override ILiaraViewTemplate ResolveInternalView(ILiaraContext context)
        {
            if (context.Response.Status.Code != LiaraHttpStatus.OK.Code)
            {
                var viewName = "_liara_" + context.Response.Status.Code;
                var result = new LiaraViewTemplate(viewName, null, true);
                if (service.HasTemplate(viewName))
                    return result;

                var templateString = ViewProvider.GetInternalView(context);
                service.GetTemplate(templateString, new ErrorMessage(), viewName);
                return result;
            }
            return null;
        }

        public override ILiaraViewTemplate ResolveView(ILiaraContext context)
        {
            var viewName = context.Response.Format.View;
            if (viewName == null)
            {
                if (context.Route != null)
                {
                    viewName = context.Route.Id.ToString();
                }
            }
            var result = new LiaraViewTemplate(viewName, null);

            if (service.HasTemplate(viewName))
                return result;

            var templateString = ViewProvider.GetView(context);
            if (templateString == null)
            {
                context.Response.Status = LiaraHttpStatus.NoViewAssociated;
                return ResolveInternalView(context);
            }
            service.GetTemplate(templateString, context.Response.Content, viewName);
            return result;
        }


        public override Task<string> RenderView(ILiaraViewTemplate viewTemplate, object model)
        {
            return Task.FromResult(service.Run(viewTemplate.Name, model, null));
        }
    }

    public class RazorViewProvider : LiaraViewProvider
    {
        public RazorViewProvider()
        {
            InternalViewResourceLocationString = "Liara.Formatting.Views.{0}.cshtml";
            ViewFileExtensions.Insert(0, ".cshtml");
        }

        public override string GetInternalView(ILiaraContext context)
        {
            return GetInternalViewResource("Error");
        }
    }

    public abstract class LiaraRazorTemplateBase<T> : TemplateBase<T>
    {
        public ILiaraContext Context { get; set; }
    }
}