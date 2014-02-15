// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using Liara.Common;

namespace Liara.ResponseProcessing
{
    public class LiaraResponseCookieCollection : IList<LiaraCookie>
    {
        private readonly ILiaraContext context;
        private readonly OrderedDictionary store;
        private bool hasChangedSinceLastFlush;

        public LiaraResponseCookieCollection()
        {
            store = new OrderedDictionary(StringComparer.Ordinal);
        }

        public LiaraResponseCookieCollection(ILiaraContext context) : this()
        {
            this.context = context;
        }

        public void Add(LiaraCookie cookie)
        {
            store.Add(GetInternalCookieName(cookie), cookie);
            MarkCollectionChanged();
        }

        public void Clear()
        {
            store.Clear();
            MarkCollectionChanged();
        }

        public bool Contains(LiaraCookie item)
        {
            var key = GetInternalCookieName(item);
            return store.Contains(key);
        }

        public void CopyTo(LiaraCookie[] array, int arrayIndex)
        {
            store.CopyTo(array, arrayIndex);
        }

        public bool Remove(LiaraCookie item)
        {
            if (!Contains(item))
                return false;

            store.Remove(GetInternalCookieName(item));
            MarkCollectionChanged();

            return true;
        }

        public int Count
        {
            get { return store.Count; }
        }

        public bool IsReadOnly
        {
            get { return store.IsReadOnly; }
        }

        public IEnumerator<LiaraCookie> GetEnumerator()
        {
            return (IEnumerator<LiaraCookie>) store.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(LiaraCookie item)
        {
            for (int i = 0; i < store.Count; i++)
            {
                if (item.IsSameCookie((LiaraCookie) store[i]))
                    return i;
            }
            return -1;
        }

        public void Insert(int index, LiaraCookie item)
        {
            store.Insert(index, GetInternalCookieName(item), item);
            MarkCollectionChanged();
        }

        public void RemoveAt(int index)
        {
            store.RemoveAt(index);
            MarkCollectionChanged();
        }

        public LiaraCookie this[int index]
        {
            get { return (LiaraCookie) store[index]; }
            set
            {
                store[index] = value;
                MarkCollectionChanged();
            }
        }

        private void MarkCollectionChanged()
        {
            hasChangedSinceLastFlush = true;
        }

        private void UnMarkCollectionChanged()
        {
            hasChangedSinceLastFlush = false;
        }

        /// <summary>
        ///     Provide a full cookie name with default domain name, and path.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetInternalCookieName(string key)
        {
            var cookie = new LiaraCookie();
            return cookie.Domain + cookie.Path + key;
        }

        /// <summary>
        ///     Provide a full cookie name with domain, and path in the cookie.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        private string GetInternalCookieName(LiaraCookie cookie)
        {
            return (cookie.Domain + cookie.Path).ToLowerInvariant() + cookie.Name;
        }


        public void Add(string name, string value)
        {
            var cookie = new LiaraCookie(name, value);
            store.Add(GetInternalCookieName(cookie), cookie);
            MarkCollectionChanged();
        }

        public void Add(string name, string value, DateTime expires)
        {
            var cookie = new LiaraCookie(name, value, expires);
            store.Add(GetInternalCookieName(cookie), cookie);
            MarkCollectionChanged();
        }

        public void Set(string name, string value)
        {
            var cookie = new LiaraCookie(name, value);
            store[GetInternalCookieName(cookie)] = cookie;
            MarkCollectionChanged();
        }

        public void Set(string name, string value, DateTime expires)
        {
            var cookie = new LiaraCookie(name, value, expires);
            store[GetInternalCookieName(cookie)] = cookie;
            MarkCollectionChanged();
        }

        public void Expire(string name)
        {
            var cookie = new LiaraCookie {Name = name};
            cookie.SetAsExpired();
            store[GetInternalCookieName(cookie)] = cookie;
            MarkCollectionChanged();
        }

        public void Set(LiaraCookie cookie)
        {
            store[GetInternalCookieName(cookie)] = cookie;
            MarkCollectionChanged();
        }

        public void FlushToHeaders(bool force = false)
        {
            if (hasChangedSinceLastFlush || force)
            {
                foreach (var cookie in this)
                {
                    AddCookieToHeader(cookie);
                }

                UnMarkCollectionChanged();
            }
        }

        private void AddCookieToHeader(LiaraCookie cookie)
        {
            bool domainHasValue = !string.IsNullOrEmpty(cookie.Domain);
            bool pathHasValue = !string.IsNullOrEmpty(cookie.Path);
            bool expiresHasValue = cookie.Expires.HasValue;

            string setCookieValue = string.Concat(
                Uri.EscapeDataString(cookie.Name),
                "=",
                Uri.EscapeDataString(cookie.Value ?? string.Empty),
                domainHasValue ? "; domain=" : null,
                domainHasValue ? cookie.Domain : null,
                pathHasValue ? "; path=" : null,
                pathHasValue ? cookie.Path : null,
                expiresHasValue
                    ? "; expires=" +
                      cookie.Expires.Value.ToString("ddd, dd-MMM-yyyy HH:mm:ss ", CultureInfo.InvariantCulture) + "GMT"
                    : null,
                cookie.SecureOnly ? "; secure" : null,
                cookie.HttpOnly ? "; HttpOnly" : null);
            context.Response.Headers.AppendValue(Constants.ResponseHeaderConstants.SetCookie, setCookieValue);
        }
    }
}