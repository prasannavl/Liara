// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System.Collections.Generic;
using System.Dynamic;

namespace Liara.Common
{
    public interface ILiaraHashTable : IDynamicMetaObjectProvider, IDictionary<string, string[]>
    {
        string Get(string key);
        string[] GetValues(string key);
        void Set(string key, string value);
        void SetValues(string key, string[] values);
        void AppendValue(string key, string value, bool createIfKeyIsNotPresent = true);
        void AppendValues(string key, string[] values, bool createIfKeyIsNotPresent = true);
    }
}