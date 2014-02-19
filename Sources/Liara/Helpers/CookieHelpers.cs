// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Collections.Generic;
using Liara.Common;
using Liara.Helpers;

namespace Liara
{
    internal static class CookieHelpers
    {
        private static readonly Action<string, string, object> AddCookieCallback = (name, value, state) =>
        {
            var dictionary = (IDictionary<string, string>) state;
            dictionary[name] = value;
        };

        private static readonly char[] SemicolonAndComma = {';', ','};

        /// <summary>
        ///     Extract Request cookies from the header.
        ///     <para>Cookies are case-sensitive in most scenarios.</para>
        ///     <para>
        ///         But since the RFC standard doesn't enforce it, the <paramref name="isCaseSensitive" /> can be used to change
        ///         behaviour.
        ///     </para>
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="isCaseSensitive"></param>
        /// <returns></returns>
        internal static IDictionary<string, string> ParseRequestCookies(ILiaraHashTable<string> headers,
            bool isCaseSensitive = true)
        {
            var cookies = new Dictionary<string, string>(isCaseSensitive
                ? StringComparer.Ordinal
                : StringComparer.OrdinalIgnoreCase);
            string text = headers.Get(Constants.RequestHeaderConstants.Cookie);
            StringHelpers.ParseUrlEncodedString(text, SemicolonAndComma, AddCookieCallback, cookies);
            return cookies;
        }
    }
}