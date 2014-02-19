// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Razor.Editor;

namespace Liara.Common
{
    public class QValuedMediaType : MediaType, IEquatable<QValuedMediaType>, IQValuedItem
    {
        public new static readonly QValuedMediaType Any = new QValuedMediaType {Group = "*", Type = "*"};

        public QValuedMediaType()
        {
            Initialize();
        }

        public QValuedMediaType(string mediaTypeString, float quality = 1)
            : base(mediaTypeString)
        {
            Initialize();
            QValue = quality;
        }

        public QValuedMediaType(string group, string type, float quality = 1)
            : base(group, type)
        {
            Initialize();
            QValue = quality;
        }


        public List<string> ExtendedTokens { get; set; }

        public bool Equals(QValuedMediaType other)
        {
            return 
                base.Equals(other) &&
                other.ExtendedTokens.SequenceEqual(ExtendedTokens, StringComparer.OrdinalIgnoreCase) &&
                other.QValue == QValue;
        }

        public LiaraHeaderQualityFactor QValue { get; set; }

        public void Initialize()
        {
            ExtendedTokens = new List<string>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder(Group + "/" + Type);
            if (QValue < 1) sb.Append(";q=" + QValue);
            if (ExtendedTokens.Count > 0) sb.Append(";" + String.Join(";", ExtendedTokens));

            return sb.ToString();
        }

    }
}