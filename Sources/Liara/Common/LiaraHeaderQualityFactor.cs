// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Collections.Generic;

namespace Liara.Common
{
    public struct LiaraHeaderQualityFactor : IComparable<LiaraHeaderQualityFactor>
    {
        private readonly float q;

        public LiaraHeaderQualityFactor(float q)
        {
            if (q > 1 || q <= 0) throw new ArgumentOutOfRangeException("q");
            this.q = q;
        }

        public int CompareTo(LiaraHeaderQualityFactor other)
        {
            return Comparer<float>.Default.Compare(q, other.q);
        }

        public static implicit operator float(LiaraHeaderQualityFactor q)
        {
            return q.q;
        }

        public static implicit operator LiaraHeaderQualityFactor(float q)
        {
            return new LiaraHeaderQualityFactor(q);
        }
    }
}