// Author: Prasanna V. Loganathar
// Project: Liara.Hosting.Owin
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Liara.Common;
using Liara.Extensions;

namespace Liara.Hosting.Owin
{
    public class OwinServerEnvironment : ILiaraServerEnvironment
    {
        private OwinServerEnvCache cache;
        private bool disposed;

        public OwinServerEnvironment(IDictionary<string, object> environment)
        {
            Items = environment;
            cache = new OwinServerEnvCache(this);
        }

        public OwinServerEnvironment(IDictionary<string, object> environment, ILiaraEngine engine) : this(environment)
        {
            Engine = engine;
        }

        public ILiaraEngine Engine { get; set; }

        public Uri Uri
        {
            get { return cache.Uri; }
            set { cache.Uri = value; }
        }

        public string RequestPath
        {
            get { return Items.Get<string>(Constants.OwinConstants.RequestPath); }
            set { Items.Set(Constants.OwinConstants.RequestPath, value); }
        }

        public string RequestPathBase
        {
            get { return Items.Get<string>(Constants.OwinConstants.RequestPathBase); }
            set { Items.Set(Constants.OwinConstants.RequestPathBase, value); }
        }

        public string RequestProtocol
        {
            get { return Items.Get<string>(Constants.OwinConstants.RequestProtocol); }
            set { Items.Set(Constants.OwinConstants.RequestProtocol, value); }
        }

        public string RequestMethod
        {
            get { return Items.Get<string>(Constants.OwinConstants.RequestMethod); }
            set { Items.Set(Constants.OwinConstants.RequestMethod, value); }
        }

        public ILiaraHashTable RequestHeaders
        {
            get { return cache.RequestHeaders; }
            set { cache.RequestHeaders = value; }
        }

        public LiaraStream RequestBody
        {
            get { return cache.RequestBody; }
            set { cache.RequestBody = value; }
        }

        public int ResponseStatusCode
        {
            get { return Items.Get<int>(Constants.OwinConstants.ResponseStatusCode); }
            set { Items.Set(Constants.OwinConstants.ResponseStatusCode, value); }
        }

        public string ResponseStatusDescription
        {
            get { return Items.Get<string>(Constants.OwinConstants.ResponseReasonPhrase); }
            set { Items.Set(Constants.OwinConstants.ResponseReasonPhrase, value); }
        }

        public string ResponseProtocol
        {
            get { return Items.Get<string>(Constants.OwinConstants.ResponseProtocol); }
            set { Items.Set(Constants.OwinConstants.ResponseProtocol, value); }
        }

        public ILiaraHashTable ResponseHeaders
        {
            get { return cache.ResponseHeaders; }
            set { cache.ResponseHeaders = value; }
        }

        public LiaraStream ResponseBody
        {
            get { return cache.ResponseBody; }
            set { cache.ResponseBody = value; }
        }

        public string ServerIpAddress
        {
            get { return Items.Get<string>(Constants.OwinConstants.ServerLocalIpAddress); }
            set { Items.Set(Constants.OwinConstants.ServerLocalIpAddress, value); }
        }

        public int ServerPort
        {
            get { return int.Parse(Items.Get<string>(Constants.OwinConstants.ServerLocalPort)); }
            set { Items.Set(Constants.OwinConstants.ServerLocalPort, value.ToString()); }
        }

        public string ClientIpAddress
        {
            get { return Items.Get<string>(Constants.OwinConstants.ServerRemoteIpAddress); }
            set { Items.Set(Constants.OwinConstants.ServerRemoteIpAddress, value); }
        }

        public int ClientPort
        {
            get { return int.Parse(Items.Get<string>(Constants.OwinConstants.ServerRemotePort)); }
            set { Items.Set(Constants.OwinConstants.ServerRemotePort, value.ToString()); }
        }

        public X509Certificate ClientCertificate
        {
            get { return Items.Get<X509Certificate>(Constants.OwinConstants.SslClientCertifiate); }
            set { Items.Set(Constants.OwinConstants.SslClientCertifiate, value); }
        }

        public IDictionary<string, object> Items { get; set; }

        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                disposed = true;
                RequestBody.Dispose();
                ResponseBody.Dispose();
            }
        }
    }
}