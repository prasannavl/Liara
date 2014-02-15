// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;

namespace Liara.Common
{
    public class MediaType : IEquatable<MediaType>
    {
        public static readonly MediaType Any = new MediaType {group = "*", type = "*"};
        public static readonly MediaType None = new MediaType();

        protected string group;
        protected string type;

        internal MediaType()
        {
        }

        public MediaType(string group, string type)
        {
            Group = group;
            Type = type;
        }

        public MediaType(string mediaTypeString)
        {
            var split = mediaTypeString.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
            {
                throw new ArgumentException("mediaTypeString given is not a valid MediaType string");
            }
            Group = split[0];
            Type = split[1];
        }

        public string Group
        {
            get { return group; }
            internal set
            {
                if (String.IsNullOrWhiteSpace(value)) throw new ArgumentException("mediaGroup cannot be empty");
                group = value;
            }
        }

        public string Type
        {
            get { return type; }
            internal set
            {
                if (String.IsNullOrWhiteSpace(value)) throw new ArgumentException("mediaType cannot be empty");
                type = value;
            }
        }

        public bool Equals(MediaType other)
        {
            return
                String.Equals(other.Group, Group, StringComparison.OrdinalIgnoreCase) &&
                String.Equals(other.Type, Type, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsCompatible(MediaType other)
        {
            return IsGroupCompatible(other) && IsTypeCompatible(other);
        }

        public bool IsGroupCompatible(MediaType other)
        {
            return other.Group == Any.Group || Group == Any.Group ||
                   other.Group.Equals(Group, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsTypeCompatible(MediaType other)
        {
            return other.Type == Any.Type || Type == Any.Type ||
                   other.Type.Equals(Type, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsGroupExplicit()
        {
            return Group != "*";
        }

        public bool IsTypeExplicit()
        {
            return Type != "*";
        }

        public bool IsExplict()
        {
            return IsGroupExplicit() && IsTypeExplicit();
        }


        public override string ToString()
        {
            return Group + "/" + Type;
        }
    }
}