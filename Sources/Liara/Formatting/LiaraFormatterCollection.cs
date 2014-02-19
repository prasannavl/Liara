// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 1:39 AM 18-02-2014

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Liara.Common;

namespace Liara.Formatting
{
    public class LiaraFormatterCollection : ILiaraPrioritizedServiceCollection<ILiaraFormatter>
    {
        private IList<ILiaraFormatter> store;

        public LiaraFormatterCollection()
        {
            store = new List<ILiaraFormatter>();
            Initialize();
        }

        public LiaraFormatterCollection(IList<ILiaraFormatter> store)
        {
            this.store = store;
            Initialize();

            foreach (var liaraFormatter in store)
            {
                AddToMaps(liaraFormatter);
            }
        }

        public ILiaraHashTable<ILiaraFormatter> UrlMap { get; set; }
        public ILiaraHashTable<ILiaraFormatter> MediaMap { get; set; }

        public IEnumerator<ILiaraFormatter> GetEnumerator()
        {
            return store.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) store).GetEnumerator();
        }

        public void Add(ILiaraFormatter item)
        {
            store.Add(item);
            AddToMaps(item);
        }

        public void Clear()
        {
            foreach (var liaraFormatter in store)
            {
                Remove(liaraFormatter);
            }
        }

        public bool Contains(ILiaraFormatter item)
        {
            return store.Contains(item);
        }

        public void CopyTo(ILiaraFormatter[] array, int arrayIndex)
        {
            store.CopyTo(array, arrayIndex);
        }

        public bool Remove(ILiaraFormatter item)
        {
            RemoveFromMaps(item);
            return store.Remove(item);
        }

        public int Count
        {
            get { return store.Count; }
        }

        public bool IsReadOnly
        {
            get { return store.IsReadOnly; }
        }

        public int IndexOf(ILiaraFormatter item)
        {
            return store.IndexOf(item);
        }

        public void Insert(int index, ILiaraFormatter item)
        {
            store.Insert(index, item);
            AddToMaps(item);
        }

        public void RemoveAt(int index)
        {
            var item = store.ElementAt(index);
            Remove(item);
        }

        public ILiaraFormatter this[int index]
        {
            get { return store[index]; }
            set
            {
                var existing = store.ElementAtOrDefault(index);
                if (existing != null)
                {
                    RemoveFromMaps(existing);
                }
                Insert(index, value);
            }
        }

        private void Initialize()
        {
            MediaMap = new LiaraHashTable<ILiaraFormatter>();
            UrlMap = new LiaraHashTable<ILiaraFormatter>();
        }

        private void AddToMaps(ILiaraFormatter item)
        {
            if (item.GetType().IsSubclassOf(typeof (LiaraFormatter)))
            {
                var fm = (LiaraFormatter) item;
                foreach (var supportedMediaType in fm.SupportedMediaTypes)
                {
                    MediaMap.AppendValue(
                        supportedMediaType.ToString().ToLower(),
                        item);
                }

                foreach (var supportedUrlExtension in fm.SupportedUrlExtensions)
                {
                    UrlMap.AppendValue(
                        supportedUrlExtension.ToLower(),
                        item);
                }
            }
        }

        private void RemoveFromMaps(ILiaraFormatter item)
        {
            if (item.GetType().IsSubclassOf(typeof (LiaraFormatter)))
            {
                var fm = (LiaraFormatter) item;
                foreach (var supportedMediaType in fm.SupportedMediaTypes)
                {
                    MediaMap.RemoveValue(supportedMediaType.ToString().ToLower(), item);
                }

                foreach (var supportedUrlExtension in fm.SupportedUrlExtensions)
                {
                    UrlMap.RemoveValue(
                        supportedUrlExtension.ToLower(),
                        item);
                }
            }
        }
    }
}