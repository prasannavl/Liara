// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:54 AM 15-02-2014

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Liara.Common;

namespace Liara
{
    public interface ILiaraServerEnvironment : IDisposable
    {
        Uri Uri { get; set; }
        string RequestPath { get; set; }
        string RequestPathBase { get; set; }
        string RequestProtocol { get; set; }
        string RequestMethod { get; set; }
        ILiaraHashTable RequestHeaders { get; set; }
        LiaraStream RequestBody { get; set; }


        int ResponseStatusCode { get; set; }
        string ResponseStatusDescription { get; set; }
        string ResponseProtocol { get; set; }
        ILiaraHashTable ResponseHeaders { get; set; }
        LiaraStream ResponseBody { get; set; }


        string ServerIpAddress { get; set; }
        int ServerPort { get; set; }
        string ClientIpAddress { get; set; }
        int ClientPort { get; set; }
        X509Certificate ClientCertificate { get; set; }

        IDictionary<string, object> Items { get; set; }
    }
}