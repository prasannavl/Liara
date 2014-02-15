// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Liara.Constants;

namespace Liara.RequestProcessing
{
    public class LiaraRequestInfo
    {
        private readonly ILiaraContext context;

        public LiaraRequestInfo(ILiaraContext context)
        {
            this.context = context;
        }

        public Uri Uri
        {
            get { return context.Environment.Uri; }
        }

        public string Host
        {
            get { return Uri.Host; }
        }

        public string Path
        {
            get { return context.Environment.RequestPath; }
        }

        public string PathBase
        {
            get { return context.Environment.RequestPathBase; }
        }

        public string Protocol
        {
            get { return context.Environment.RequestProtocol; }
        }

        public Version HttpVersion
        {
            get { return Version.Parse(context.Environment.RequestProtocol.Split('/')[1]); }
        }

        public bool IsAjax
        {
            get
            {
                var xreq = context.Environment.RequestHeaders.GetValues(Constants.RequestHeaderConstants.XRequestedWith);
                return xreq != null && xreq.Contains(CommonConstants.XmlHttpRequest);
            }
        }

        public string Method
        {
            get { return context.Environment.RequestMethod; }
        }

        public string Scheme
        {
            get { return Uri.Scheme; }
        }

        public bool IsLocal
        {
            get { return Uri.IsLoopback; }
        }

        public bool IsSecure
        {
            get
            {
                return string.Equals(Scheme.ToLowerInvariant(), Constants.CommonConstants.Https,
                    StringComparison.Ordinal);
            }
        }

        public string ServerIpAddress
        {
            get { return context.Environment.ServerIpAddress; }
        }

        public string ClientIpAddress
        {
            get { return context.Environment.ClientIpAddress; }
        }

        public int ServerPort
        {
            get { return context.Environment.ServerPort; }
        }

        public int ClientPort
        {
            get { return context.Environment.ClientPort; }
        }

        public X509Certificate ClientCertificate
        {
            get { return context.Environment.ClientCertificate; }
        }
    }
}