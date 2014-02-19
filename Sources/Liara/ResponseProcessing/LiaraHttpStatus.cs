// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

using System;
using System.Collections.Generic;
using Liara.ResponseProcessing;

namespace Liara
{
    /// <summary>
    ///     Http status codes.
    /// </summary>
    public sealed partial class LiaraHttpStatus
    {
        private static readonly Dictionary<int, LiaraHttpStatus> Instance = new Dictionary<int, LiaraHttpStatus>();
        public readonly HttpStatusCategory Category = HttpStatusCategory.Uncategorized;

        public readonly int Code;
        public readonly string Description;

        public LiaraHttpStatus(int code, string description)
        {
            Code = code;
            Description = description;
        }

        internal LiaraHttpStatus(int code, string description, HttpStatusCategory category)
        {
            Code = code;
            Description = description;
            Instance[code] = this;
        }

        public override string ToString()
        {
            return Description;
        }

        public static implicit operator LiaraHttpStatus(int statusCode)
        {
            LiaraHttpStatus status;
            if (Instance.TryGetValue(statusCode, out status))
                return status;
            throw new InvalidCastException();
        }
    }
}