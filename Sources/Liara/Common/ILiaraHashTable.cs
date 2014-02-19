// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

using System.Collections.Generic;
using System.Dynamic;

namespace Liara.Common
{
    public interface ILiaraHashTable<T> : IDynamicMetaObjectProvider, IDictionary<string, T[]>
    {
        T Get(string key);
        T[] GetValues(string key);
        void Set(string key, T value);
        void SetValues(string key, T[] values);
        void AppendValue(string key, T value, bool createIfKeyIsNotPresent = true);
        void AppendValues(string key, T[] values, bool createIfKeyIsNotPresent = true);
        void RemoveValue(string key, T value, bool deleteKeyIfLastElement = true);
        void RemoveValues(string key, T[] values, bool deleteKeyIfLastElement = true);
    }
}