// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Liara.Common
{
    public class LiaraStringHashTable : DynamicObject, ILiaraHashTable<string>
    {
        protected IDictionary<string, string[]> store;

        public LiaraStringHashTable(bool isCaseSensitive = true)
        {
            store =
                new Dictionary<string, string[]>(isCaseSensitive
                    ? StringComparer.Ordinal
                    : StringComparer.OrdinalIgnoreCase);
        }

        public LiaraStringHashTable(IDictionary<string, string[]> existingCollection)
        {
            store = existingCollection;
        }


        public void Add(string key, string[] value)
        {
            store.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return store.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return store.Keys; }
        }

        public bool Remove(string key)
        {
            return store.Remove(key);
        }

        public bool TryGetValue(string key, out string[] value)
        {
            return store.TryGetValue(key, out value);
        }

        public ICollection<string[]> Values
        {
            get { return store.Values; }
        }

        public string[] this[string key]
        {
            get { return store[key]; }
            set { store[key] = value; }
        }

        public void Add(KeyValuePair<string, string[]> item)
        {
            store.Add(item);
        }

        public void Clear()
        {
            store.Clear();
        }

        public bool Contains(KeyValuePair<string, string[]> item)
        {
            return store.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string[]>[] array, int arrayIndex)
        {
            store.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return store.Count; }
        }

        public bool IsReadOnly
        {
            get { return store.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, string[]> item)
        {
            return store.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, string[]>> GetEnumerator()
        {
            return store.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string Get(string key)
        {
            string[] values;
            return TryGetValue(key, out values) ? String.Join(",", values) : null;
        }

        public string[] GetValues(string key)
        {
            string[] values;
            return TryGetValue(key, out values) ? values : null;
        }

        public void Set(string key, string value)
        {
            if (value == null)
            {
                this[key] = new string[] {};
            }
            else
            {
                this[key] = new[] {value};
            }
        }

        public void SetValues(string key, string[] values)
        {
            if (values == null)
            {
                this[key] = new string[] {};
            }
            else
            {
                this[key] = values;
            }
        }

        public void AppendValue(string key, string value, bool createIfKeyIsNotPresent = true)
        {
            var existing = GetValues(key);
            if (existing == null)
            {
                if (!createIfKeyIsNotPresent)
                    return;

                Set(key, value);
            }
            else if (value != null)
            {
                var len = existing.Length;
                Array.Resize(ref existing, len + 1);
                existing[len] = value;
            }
        }

        public void AppendValues(string key, string[] values, bool createIfKeyIsNotPresent = true)
        {
            var existing = GetValues(key);
            if (existing == null)
            {
                if (!createIfKeyIsNotPresent)
                    return;

                SetValues(key, values);
            }
            else if (values != null && values.Length > 0)
            {
                var len = existing.Length;
                Array.Resize(ref existing, len + values.Length);
                for (int i = 0; i < values.Length; i++)
                {
                    existing[len + i] = values[i];
                }
            }
        }

        public void RemoveValue(string key, string value, bool deleteKeyIfLastElement = true)
        {
            var existing = GetValues(key);
            if (existing != null)
            {
                if (value != null)
                {
                    existing = existing.Where(val => !val.Equals(value)).ToArray();
                    store[key] = existing;
                }

                if (deleteKeyIfLastElement && !existing.Any())
                    store.Remove(key);
            }
        }

        public void RemoveValues(string key, string[] values, bool deleteKeyIfLastElement = true)
        {
            var existing = GetValues(key);
            if (existing != null)
            {
                if (values != null && values.Any())
                {
                    existing = existing.Where(val => !values.Contains(val)).ToArray();
                    store[key] = existing;
                }

                if (deleteKeyIfLastElement && !existing.Any())
                    store.Remove(key);
            }
        }

        /// <summary>
        ///     Provides the implementation for operations that set member values. Classes derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for
        ///     operations such as setting a value for a property.
        /// </summary>
        /// <returns>
        ///     true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of
        ///     the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        /// <param name="binder">
        ///     Provides information about the object that called the dynamic operation. The binder.Name property
        ///     provides the name of the member to which the value is being assigned. For example, for the statement
        ///     sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase
        ///     property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="value">
        ///     The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where
        ///     sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the
        ///     <paramref name="value" /> is "Test".
        /// </param>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Set(binder.Name, value.ToString());
            return true;
        }

        /// <summary>
        ///     Provides the implementation for operations that get member values. Classes derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for
        ///     operations such as getting a value for a property.
        /// </summary>
        /// <returns>
        ///     true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of
        ///     the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        /// <param name="binder">
        ///     Provides information about the object that called the dynamic operation. The binder.Name property
        ///     provides the name of the member on which the dynamic operation is performed. For example, for the
        ///     Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived
        ///     from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The
        ///     binder.IgnoreCase property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="result">
        ///     The result of the get operation. For example, if the method is called for a property, you can
        ///     assign the property value to <paramref name="result" />.
        /// </param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string[] typeBuffer;
            var hasValue = TryGetValue(binder.Name, out typeBuffer);
            if (hasValue)
            {
                result = typeBuffer;
                return true;
            }

            result = null;
            return false;
        }
    }
}