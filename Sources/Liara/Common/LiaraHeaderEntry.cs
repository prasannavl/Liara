// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

namespace Liara.Common
{
    public class LiaraHeaderEntry
    {
        private readonly string fieldName;
        private readonly ILiaraHashTable<string> headers;

        public LiaraHeaderEntry(ILiaraHashTable<string> headers, string headerName)
        {
            this.headers = headers;
            fieldName = headerName;
        }

        public string FieldName
        {
            get { return fieldName; }
        }

        public string Value
        {
            get { return headers.Get(FieldName); }
            set { headers.Set(FieldName, value); }
        }

        public string[] Values
        {
            get { return headers.GetValues(FieldName); }
            set { headers.SetValues(FieldName, value); }
        }

        public bool HasValue
        {
            get { return headers.ContainsKey(FieldName); }
        }
    }
}