// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System.Collections.Generic;

namespace Liara.Common
{
    public interface IQValuedItem
    {
        LiaraHeaderQualityFactor QValue { get; set; }
    }

    public class LiaraQValuedHeader : IQValuedItem
    {
        public string Value { get; set; }
        public LiaraHeaderQualityFactor QValue { get; set; }
    }

    public class LiaraQValuedMultiItemHeader : IQValuedItem
    {
        public LiaraQValuedMultiItemHeader()
        {
            Values = new List<string>();
        }

        public List<string> Values { get; set; }
        public LiaraHeaderQualityFactor QValue { get; set; }
    }
}