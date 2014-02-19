// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

using System;

namespace Liara.Common
{
    [Serializable]
    public class ErrorMessage
    {
        public int ErrorCode { get; set; }
        public string Description { get; set; }
        public object Message { get; set; }
    }
}