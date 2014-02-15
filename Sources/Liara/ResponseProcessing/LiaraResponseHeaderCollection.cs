// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using Liara.Common;

namespace Liara.ResponseProcessing
{
    public sealed partial class LiaraResponseHeaderCollection : LiaraHashTable
    {
        public LiaraResponseHeaderCollection() : base(false)
        {
        }

        public LiaraResponseHeaderCollection(ILiaraContext context) : base(context.Environment.ResponseHeaders)
        {
        }
    }
}