// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

using System;
using System.IO;

namespace Liara.RequestProcessing
{
    public class LiaraRequestParameters
    {
        private readonly ILiaraContext context;
        private ContentDeterminationType internalContentType;
        private bool isRequestContentAvailabilitySet;
        private bool isRequestContentAvailable;
        private Type typeCache;

        public LiaraRequestParameters(ILiaraContext context)
        {
            this.context = context;
            if (context.Route.RequestModel != null)
            {
                isRequestContentAvailable = isRequestContentAvailabilitySet = true;
                internalContentType = ContentDeterminationType.Property;
            }
            else
            {
                internalContentType = ContentDeterminationType.Unknown;
            }
        }

        private Type TypeCache
        {
            get { return typeCache ?? (typeCache = context.Route.RequestModel ?? context.Request.Content.GetType()); }
        }

        private bool IsRequestContentAvailable
        {
            get
            {
                if (!isRequestContentAvailabilitySet)
                {
                    isRequestContentAvailable = !(context.Request.Content is Stream);
                    isRequestContentAvailabilitySet = true;
                }
                return isRequestContentAvailable;
            }
        }

        public dynamic this[string name]
        {
            get { return Get(name); }
        }

        //TODO: Model State.
        public object ModelState { get; set; }

        public virtual dynamic Get(string key)
        {
            return GetValueInternal(key);
        }

        public string GetFromQuery(string key)
        {
            return context.Request.QueryString.Get(key);
        }

        public string GetFromRoute(string key)
        {
            string result;
            context.Route.Parameters.TryGetValue(key, out result);
            return result;
        }

        public object GetFromContent(string key)
        {
            switch (internalContentType)
            {
                case ContentDeterminationType.Unknown:
                {
                    if (IsRequestContentAvailable)
                        goto case ContentDeterminationType.Dictionary;
                    internalContentType = ContentDeterminationType.None;
                    return null;
                }
                case ContentDeterminationType.Dictionary:
                {
                    try
                    {
                        var result = context.Request.Content[key];
                        internalContentType = ContentDeterminationType.Dictionary;
                        return result;
                    }
                    catch
                    {
                        goto case ContentDeterminationType.Property;
                    }
                }
                case ContentDeterminationType.Property:
                {
                    try
                    {
                        var prop = TypeCache.GetProperty(key);
                        var result = prop.GetValue(context.Request.Content);
                        internalContentType = ContentDeterminationType.Property;
                        return result;
                    }
                    catch
                    {
                        internalContentType = ContentDeterminationType.Dictionary;
                    }
                    break;
                }
                default:
                {
                    return null;
                }
            }
            return null;
        }

        private dynamic GetValueInternal(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            dynamic result = GetFromRoute(key);
            if (result != null)
                return result;
            result = GetFromQuery(key);
            if (result != null)
                return result;
            result = GetFromContent(key);

            return result;
        }

        //TODO: Model Binding
        public object Bind()
        {
            throw new NotImplementedException();
        }

        //TODO: Model Binding
        public T Bind<T>()
        {
            throw new NotImplementedException();
        }

        //TODO: Model Binding
        public object BindAndValidate()
        {
            throw new NotImplementedException();
        }


        //TODO: Model Validation
        public bool Validate()
        {
            throw new NotImplementedException();
        }

        private enum ContentDeterminationType
        {
            Unknown,
            Dictionary,
            Property,
            None
        }
    }
}