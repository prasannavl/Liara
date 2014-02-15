// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 2:39 PM 13-02-2014

using System;
using System.Collections.Generic;

namespace Liara.Services
{
    public interface ILiaraServicesContainer : IDisposable
    {
        void Register(Type serviceType, object instance, string serviceName);
        void Register(Type serviceType, Type implementingType, string serviceName, LiaraServiceLifeTime lifeTime);
        ILiaraServicesContainer GetChildScope();
        object GetContainer();
        object Get(Type serviceType);
        object Get(Type serviceType, string serviceName);
        T Get<T>();
        T Get<T>(string serviceName);
        IEnumerable<object> GetAll(Type serviceType);
        IEnumerable<T> GetAll<T>();
    }
}