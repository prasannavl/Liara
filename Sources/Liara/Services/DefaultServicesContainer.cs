// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public void Register(Type serviceType, object instance, string serviceName)
        {
            serviceContainer.RegisterInstance(serviceType, instance, serviceName);
        }

        public void Register(Type serviceType, Type implementingType, string serviceName, LiaraServiceLifeTime lifeTime)
        {
            if (lifeTime == LiaraServiceLifeTime.Singleton)
            {
                serviceContainer.Register(serviceType, implementingType, serviceName, new PerContainerLifetime());
            }
            else if (lifeTime == LiaraServiceLifeTime.Transient)
            {
                serviceContainer.Register(serviceType, implementingType, serviceName);
            }
            else if (lifeTime == LiaraServiceLifeTime.PerRequest)
            {
                serviceContainer.Register(serviceType, implementingType, serviceName, new PerScopeLifetime());
            }
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