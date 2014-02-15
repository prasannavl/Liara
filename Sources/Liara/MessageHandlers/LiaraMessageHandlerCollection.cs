// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Liara.MessageHandlers
{
    /// <summary>
    ///     LiaraMessageHandler Collection that auto-links innerhandlers
    ///     and maintains a valid chain.
    ///     <para>
    ///         Note: This collection is not thread-safe to provide maximum performance.
    ///         <para>
    ///             Utilize proper locking, if the collection is dynamically modified after Liara Engine Initialization.
    ///         </para>
    ///     </para>
    /// </summary>
    public class LiaraMessageHandlerCollection : IList<LiaraMessageHandler>
    {
        public LiaraMessageHandlerCollection()
        {
            Store = new List<LiaraMessageHandler>();
        }

        public LiaraMessageHandlerCollection(IList<LiaraMessageHandler> collection)
        {
            Store = collection;
        }

        private IList<LiaraMessageHandler> Store { get; set; }

        public int IndexOf(LiaraMessageHandler item)
        {
            return Store.IndexOf(item);
        }

        public void Insert(int index, LiaraMessageHandler item)
        {
            Store.Insert(index, item);
            Relink(index);
        }

        public void RemoveAt(int index)
        {
            Store.ElementAt(index).Dispose();
            Store.RemoveAt(index);
            RelinkPrevious(--index);
        }

        public LiaraMessageHandler this[int index]
        {
            get { return Store[index]; }
            set
            {
                Store[index] = value;
                Relink(index);
            }
        }

        public void Add(LiaraMessageHandler item)
        {
            var index = Store.Count;
            Store.Add(item);
            Relink(index);
        }

        public void Clear()
        {
            Store.Clear();
        }

        public bool Contains(LiaraMessageHandler item)
        {
            return Store.Contains(item);
        }

        public void CopyTo(LiaraMessageHandler[] array, int arrayIndex)
        {
            Store.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Store.Count(); }
        }

        public bool IsReadOnly
        {
            get { return Store.IsReadOnly; }
        }

        public bool Remove(LiaraMessageHandler item)
        {
            var index = Store.IndexOf(item);
            if (index > 0)
            {
                RemoveAt(index);
            }
            return false;
        }

        public IEnumerator<LiaraMessageHandler> GetEnumerator()
        {
            return Store.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Task Execute(LiaraContext context)
        {
            return Store.Count > 0 ? Store[0].ProcessAsync(context) : TaskHelpers.Completed();
        }

        private void Relink(int index)
        {
            RelinkPrevious(index);
            LinkCurrent(index);
        }

        private void LinkCurrent(int index)
        {
            var i = index + 1;
            if (i > -1 && i < Store.Count)
            {
                Store.ElementAt(index).InnerHandler = Store.ElementAt(i);
            }
        }

        private void RelinkPrevious(int index)
        {
            var i = index - 1;
            if (i > -1 && i < Store.Count)
            {
                if (index < Store.Count)
                {
                    Store.ElementAt(i).InnerHandler = Store.ElementAt(index);
                }
                else
                {
                    Store.ElementAt(i).InnerHandler = null;
                }
            }
        }
    }
}