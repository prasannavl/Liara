// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using Liara.Common;
using Liara.Helpers;

namespace Liara.RequestProcessing
{
    public class LiaraQueryString : LiaraStringHashTable
    {
        public LiaraQueryString() : base(false)
        {
        }

        public LiaraQueryString(ILiaraContext context)
        {
            base.store = QueryStringHelpers.ParseFromString(context.Request.Info.Uri.Query);
        }

        /// <summary>
        ///     Return the correctly formed and escaped query string that is can directly be added
        ///     to the Uri component, including the leading "?".
        /// </summary>
        /// <returns>Correct query string, with the leading "?"</returns>
        public override string ToString()
        {
            return QueryStringHelpers.ConvertToUriString(this, prefixQuestionMark: true);
        }
    }
}