// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 10:45 PM 14-02-2014

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Liara.Common;
using Liara.Formatting;
using Liara.Logging;
using Liara.ResponseProcessing;
using LightInject;

namespace Liara.Services
{
    public class DefaultServicesContainer : ILiaraServicesContainer
    {
        private readonly ConcurrentStack<Scope> scopes = new ConcurrentStack<Scope>();
        private readonly IServiceContainer serviceContainer;

        public DefaultServicesContainer()
        {
            serviceContainer = new ServiceContainer();
        }

        public DefaultServicesContainer(ServiceContainer serviceContainer)
        {
            if (serviceContainer == null) throw new ArgumentNullException("serviceContainer");
            this.serviceContainer = serviceContainer;
        }

        public void Dispose()
        {
            Scope scope;
            var isInScope = scopes.TryPop(out scope);
            if (isInScope)
            {
                scope.Dispose();
            }
        }

        public virtual void Discover()
        {
            var blackList = new List<string>
            {
                "mscorlib",
                "System",
                "System.*",
                "Microsoft.*",
                "LightInject",
            };

            var whiteList = new List<string>();

            var configurableTypes = new List<Type>
            {
                typeof (LiaraModule),
                typeof (ILiaraFormatSelector),
                typeof (ILiaraFormatter),
                typeof (ILiaraStatusHandler),
                typeof (ILiaraResponseSynchronizer),
                typeof (ILiaraLogWriter)
            };

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var liaraAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var liaraExtendedAssemblies = liaraAssemblyName + ".*";

            foreach (var assembly in assemblies)
            {
                var name = assembly.GetName().Name;
                if (whiteList.Any(w => IsWildcardedMatch(w, name)) ||
                    !blackList.Any(b => IsWildcardedMatch(b, name)))
                {
                    if (name == liaraAssemblyName || IsWildcardedMatch(liaraExtendedAssemblies, name))
                    {
                        serviceContainer.RegisterAssembly(assembly, (type, implType) => configurableTypes.Contains(type));
                    }
                    else
                    {
                        serviceContainer.RegisterAssembly(assembly);
                    }
                }
            }

            if (LiaraEngine.FrameworkLogger.IsEnabled)
            {
                foreach (var serviceRegistration in serviceContainer.AvailableServices)
                {
                    LiaraEngine.FrameworkLogger.WriteTo("Registered Services", "Name : {0}, Type: {1}",
                        serviceRegistration.ServiceName, serviceRegistration.ServiceType);
                }
            }
        }
        public bool IsWildcardedMatch(string wildCardedString, string valueString)
        {
            if (wildCardedString.Contains('*'))
            {
                return valueString.StartsWith(wildCardedString.Substring(0, wildCardedString.IndexOf('*'))) &&
                       valueString.EndsWith(wildCardedString.Substring(wildCardedString.LastIndexOf('*') + 1));
            }

            return wildCardedString == valueString;
        }

        public ILiaraServicesContainer GetChildScope()
        {
            scopes.Push(serviceContainer.BeginScope());
            return this;
        }

        public object GetContainer()
        {
            return serviceContainer;
        }

        public object Get(Type serviceType)
        {
            return serviceContainer.GetInstance(serviceType);
        }

        public object Get(Type serviceType, string serviceName)
        {
            return serviceContainer.GetInstance(serviceType, serviceName);
        }

        public T Get<T>()
        {
            return serviceContainer.GetInstance<T>();
        }

        public T Get<T>(string serviceName)
        {
            return serviceContainer.GetInstance<T>(serviceName);
        }

        public IEnumerable<object> GetAll(Type serviceType)
        {
            return serviceContainer.GetAllInstances(serviceType);
        }

        public IEnumerable<T> GetAll<T>()
        {
            return serviceContainer.GetAllInstances<T>();
        }
    }
}