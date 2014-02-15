// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Collections.Generic;

namespace Liara.RequestProcessing
{
    public class LiaraRequestCookieCollection : IDictionary<string, string>
    {
        private readonly IDictionary<string, string> store;

        /// <summary>
        ///     Construct cookies from string
        /// </summary>
        public LiaraRequestCookieCollection()
        {
            store = new Dictionary<string, string>(StringComparer.Ordinal);
        }

        public LiaraRequestCookieCollection(ILiaraContext context)
        {
            store = CookieHelpers.ParseRequestCookies(context.Environment.RequestHeaders);
        }

        public void Add(string key, string value)
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

        public bool TryGetValue(string key, out string value)
        {
            return store.TryGetValue(key, out value);
        }

        public ICollection<string> Values
        {
            get { return store.Values; }
        }

        public string this[string key]
        {
            get { return store[key]; }
            set { store[key] = value; }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            store.Add(item);
        }

        public void Clear()
        {
            store.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return store.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
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

        public bool Remove(KeyValuePair<string, string> item)
        {
            return store.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return store.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}