// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:44 PM 17-02-2014

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Liara.Helpers;
using Liara.Routing;

namespace Liara.Formatting
{
    public class LiaraViewProvider : ILiaraViewProvider
    {
        public string InternalViewResourceLocationString = "Liara.Formatting.Views.{0}.html";

        public LiaraViewProvider()
        {
            ViewLocations = new List<string>();
            var location = Assembly.GetEntryAssembly().Location;
            ViewLocations.Add(Path.Combine(Path.GetDirectoryName(location), "Views"));
            ViewFileExtensions = new List<string>();
            ViewDefaultFiles = new List<string> {"Index", "Default"};
        }

        public List<string> ViewDefaultFiles { get; set; }
        public List<string> ViewLocations { get; set; }
        public List<string> ViewFileExtensions { get; set; }

        public string GetView(ILiaraContext context)
        {
            var viewName = context.Response.Format.View;

            var route = context.Route;
            foreach (var viewLocation in ViewLocations)
            {
                foreach (var viewFileExtension in ViewFileExtensions)
                {
                    var paths = new List<string>();

                    if (viewName == null)
                    {
                        // Look for ViewPath\UrlPath{ext}
                        paths.Add(Path.Combine(viewLocation, route.Path + viewFileExtension));

                        // Look for ViewPath\UrlPath\{DefaultFiles}{ext}
                        foreach (var viewDefaultFile in ViewDefaultFiles)
                        {
                            paths.Add(Path.Combine(viewLocation, route.Path, viewDefaultFile + viewFileExtension));
                        }


                        // Look for ViewPath\ModuleName\MethodName{ext}
                        paths.Add(Path.Combine(viewLocation, route.MethodInfo.DeclaringType.Name,
                            route.MethodInfo.Name + viewFileExtension));

                        // Look for ViewPath\ModuleName\ReturnTypeName{ext}
                        // If its a generic type seperate it by '-'
                        if (route.ActionReturnType == LiaraActionReturnType.Task)
                        {
                            paths.Add(Path.Combine(viewLocation, route.MethodInfo.DeclaringType.Name,
                                string.Join("-",
                                    route.MethodInfo.ReturnType.GetGenericArguments().Select(x => x.FullName)) +
                                viewFileExtension));
                        }
                        else
                        {
                            paths.Add(Path.Combine(viewLocation, route.MethodInfo.DeclaringType.Name,
                                route.MethodInfo.ReturnType.Name + viewFileExtension));
                        }
                    }
                    else
                    {
                        paths.Add(Path.Combine(viewLocation, viewName + viewFileExtension));
                    }

                    foreach (var path in paths)
                    {
                        context.Trace.WriteToAsync("Views Searched", "{0} : {1}", context.Request.Info.Path, path);
                        if (File.Exists(path))
                            return File.ReadAllText(path);
                    }
                }
            }

            context.Trace.WriteExceptionToAsync("Unresolved Views",
                new Exception(String.Format("View not found for {0} in method: {1}",
                    context.Request.Info.Path,
                    route != null
                        ? route.MethodInfo.DeclaringType.FullName + "->" + route.MethodInfo.Name
                        : "{Unresolved}")), false);

            return null;
        }

        public virtual string GetInternalView(ILiaraContext context)
        {
            var code = context.Response.Status.Code;
            var fileName = code.ToString();
            switch (code)
            {
                case 400:
                case 404:
                case 500:
                    break;
                default:
                    if (code > 400 && code < 500)
                    {
                        fileName = "40x";
                    }
                    else if (code > 500 && code < 600)
                    {
                        fileName = "50x";
                    }
                    else
                    {
                        fileName = "Error";
                    }
                    break;
            }

            return GetInternalViewResource(fileName);
        }

        public virtual string GetInternalViewResource(string resourceFileName)
        {
            var stream =
                ReflectionHelpers.LiaraAssembly
                    .GetManifestResourceStream(string.Format(InternalViewResourceLocationString, resourceFileName));
            return new StreamReader(stream).ReadToEnd();
        }
    }
}