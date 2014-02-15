// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System;
using System.Collections.Generic;
using System.Text;
using Liara.Common;

namespace Liara.Helpers
{
    internal static class QueryStringHelpers
    {
        private static readonly char[] AmpersandAndSemicolon = {'&', ';'};

        private static readonly Action<string, string, object> AppendItemCallback = (name, value, state) =>
        {
            var queryStringCollection = (ILiaraHashTable) state;
            queryStringCollection.AppendValue(name, value);
        };

        /// <summary>
        ///     Parse the given <paramref name="text" /> into a query string.
        ///     <para>Note: By default, query strings are not case sensitive.</para>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isCaseSensitive"></param>
        /// <returns></returns>
        public static ILiaraHashTable ParseFromString(string text, bool isCaseSensitive = false)
        {
            var qs = new LiaraHashTable(isCaseSensitive);
            StringHelpers.ParseUrlEncodedString(text, AmpersandAndSemicolon, AppendItemCallback, qs);
            return qs;
        }

        /// <summary>
        ///     Return the correctly formed and escaped query string that is can directly be added
        ///     to the Uri component, including the leading "?".
        /// </summary>
        /// <returns>Correct query string, with the leading "?"</returns>
        public static string ConvertToUriString(ILiaraHashTable queryString, char seperator = '&',
            bool prefixQuestionMark = true)
        {
            if (queryString.Count < 1)
            {
                return null;
            }
            string ampChar = null;
            if (prefixQuestionMark)
            {
                ampChar = "?";
            }

            var sb = new StringBuilder(ampChar);

            var shouldAddAmpersand = false;
            foreach (KeyValuePair<string, string[]> kvp in queryString)
            {
                foreach (var itemValue in kvp.Value)
                {
                    if (shouldAddAmpersand)
                    {
                        sb.Append(seperator);
                    }
                    else
                    {
                        shouldAddAmpersand = true;
                    }

                    if (itemValue == null)
                    {
                        // Query parameter without value. Don't add '=' symbol.
                        sb.Append(Uri.EscapeDataString(kvp.Key));
                    }
                    else
                    {
                        sb.Append(string.Concat(
                            Uri.EscapeDataString(kvp.Key),
                            "=",
                            Uri.EscapeDataString(itemValue)));
                    }
                }
            }

            return sb.ToString();
        }
    }
}