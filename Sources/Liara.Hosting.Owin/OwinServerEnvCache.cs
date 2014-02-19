// Author: Prasanna V. Loganathar
// Project: Liara.Hosting.Owin
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Collections.Generic;
using System.IO;
using Liara.Common;
using Liara.Constants;
using Liara.Extensions;

namespace Liara.Hosting.Owin
{
    internal class OwinServerEnvCache
    {
        private readonly OwinServerEnvironment environment;
        private bool isRequestBodyCacheValid;

        private bool isRequestHeadersCacheValid;
        private bool isResponseBodyCacheValid;
        private bool isResponseHeadersCacheValid;
        private bool isUriCacheValid;

        private LiaraStream requestBody;
        private ILiaraHashTable<string> requestHeaders;
        private LiaraStream responseBody;
        private ILiaraHashTable<string> responseHeaders;
        private Uri uri;

        public OwinServerEnvCache(OwinServerEnvironment environment)
        {
            this.environment = environment;
        }

        public Uri Uri
        {
            get
            {
                if (!isUriCacheValid)
                {
                    var qs = environment.Items.Get<string>(OwinConstants.RequestQueryString);
                    uri = new Uri(
                        environment.Items.Get<string>(OwinConstants.RequestScheme) +
                        "://" +
                        RequestHeaders.Get(Constants.RequestHeaderConstants.Host) +
                        environment.RequestPathBase +
                        environment.RequestPath +
                        ((qs != null) ? "?" + qs : null)
                        );

                    isUriCacheValid = true;
                }
                return uri;
            }

            set
            {
                uri = value;
                isUriCacheValid = true;
            }
        }

        public ILiaraHashTable<string> RequestHeaders
        {
            get
            {
                if (!isRequestHeadersCacheValid)
                {
                    requestHeaders =
                        new LiaraStringHashTable(
                            environment.Items.Get<IDictionary<string, string[]>>(Constants.OwinConstants.RequestHeaders));
                    isRequestHeadersCacheValid = true;
                }
                return requestHeaders;
            }
            set
            {
                requestHeaders = value;
                isRequestHeadersCacheValid = true;
            }
        }

        public ILiaraHashTable<string> ResponseHeaders
        {
            get
            {
                if (!isResponseHeadersCacheValid)
                {
                    responseHeaders =
                        new LiaraStringHashTable(
                            environment.Items.Get<IDictionary<string, string[]>>(Constants.OwinConstants.ResponseHeaders));
                    isResponseHeadersCacheValid = true;
                }
                return responseHeaders;
            }
            set
            {
                responseHeaders = value;
                isResponseHeadersCacheValid = true;
            }
        }

        public LiaraStream RequestBody
        {
            get
            {
                if (!isRequestBodyCacheValid)
                {
                    requestBody =
                        new LiaraStream(environment.Items.Get<Stream>(Constants.OwinConstants.RequestBody),
                            environment.Engine.Configuration.UseBufferedRequest ? true : false);
                    isRequestBodyCacheValid = true;
                }
                return requestBody;
            }

            set
            {
                requestBody = value;
                isRequestBodyCacheValid = true;
            }
        }

        public LiaraStream ResponseBody
        {
            get
            {
                if (!isResponseBodyCacheValid)
                {
                    responseBody =
                        new LiaraStream(environment.Items.Get<Stream>(Constants.OwinConstants.ResponseBody),
                            environment.Engine.Configuration.UseBufferedResponse ? true : false);
                    isResponseBodyCacheValid = true;
                }
                return responseBody;
            }

            set
            {
                responseBody = value;
                isResponseBodyCacheValid = true;
            }
        }
    }
}