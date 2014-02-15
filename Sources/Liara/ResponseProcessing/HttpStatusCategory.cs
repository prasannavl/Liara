// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

namespace Liara.ResponseProcessing
{
    public enum HttpStatusCategory
    {
        Informational = 1,
        Success = 2,
        Redirection = 3,
        ClientError = 4,
        ServerError = 5,
        Uncategorized = 6
    };
}