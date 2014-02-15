// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

namespace Liara.RequestProcessing
{
    public class LiaraRequest : ILiaraRequest
    {
        private readonly ILiaraContext context;
        private LiaraRequestCookieCollection cookies;
        private LiaraQueryString queryString;

        public LiaraRequest(ILiaraContext context)
        {
            this.context = context;
            Format = new LiaraRequestFormatProvider(context);
        }

        public LiaraRequestInfo Info
        {
            get { return new LiaraRequestInfo(context); }
        }

        public LiaraRequestCookieCollection Cookies
        {
            get { return cookies ?? (cookies = new LiaraRequestCookieCollection(context)); }
        }

        public LiaraQueryString QueryString
        {
            get { return queryString ?? (queryString = new LiaraQueryString(context)); }
        }

        public LiaraRequestHeaderCollection Headers
        {
            get { return new LiaraRequestHeaderCollection(context); }
        }

        public LiaraRequestFormatProvider Format { get; set; }


        public LiaraRequestParameters Parameters
        {
            get { return new LiaraRequestParameters(); }
        }

        public dynamic Content { get; set; }
    }
}