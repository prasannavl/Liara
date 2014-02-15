// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Text;

namespace Liara.Helpers
{
    internal static class StringHelpers
    {
        public static string TypePrefixedString(object currentObj, string message)
        {
            return String.Concat(
                currentObj.GetType().Name,
                ": ",
                message
                );
        }

        public static string CombineWebPath(string path1, string path2)
        {
            var res = new StringBuilder();
            if (!String.IsNullOrWhiteSpace(path1))
            {
                if (!String.Equals(path1, "/", StringComparison.Ordinal) && path1.Length > 0)
                    res.Append(path1);
            }
            if (!String.IsNullOrWhiteSpace(path2))
            {
                if (!String.Equals(path2, "/", StringComparison.Ordinal) && path2.Length > 0)
                    res.Append(path2);
            }

            return res.ToString();
        }


        /// <summary>
        ///     Parse the string by delimiters, using the action callback.
        /// </summary>
        /// <param name="text">The input text</param>
        /// <param name="delimiters"></param>
        /// <param name="callback">Callback action with name, value, and state as parameters</param>
        /// <param name="state">Object state. Can be null.</param>
        public static unsafe void ParseUrlEncodedString(string text, char[] delimiters,
            Action<string, string, object> callback, object state)
        {
            var scanIndex = -1;
            fixed (char* p = text)
                while (scanIndex < text.Length)
                {
                    var delimiterIndex = text.IndexOfAny(delimiters, ++scanIndex);
                    if (delimiterIndex == scanIndex)
                    {
                        // Delimiter is the first character. Continue ahead.
                        if (p[scanIndex].Equals('+'))
                            p[scanIndex] = ' ';
                        continue;
                    }
                    if (delimiterIndex == -1)
                    {
                        // No more delimiters. Sequencing finished.
                        delimiterIndex = text.Length;
                    }
                    var wordIndex = scanIndex;
                    while (scanIndex < delimiterIndex)
                    {
                        if (p[scanIndex].Equals('='))
                        {
                            if (wordIndex == scanIndex)
                            {
                                // Possibly starts with equal symbol. Take the whole value as key.
                                wordIndex++;
                                scanIndex = delimiterIndex;
                                if (scanIndex == wordIndex)
                                {
                                    // No key, or value in this sequence.
                                    break;
                                }
                                var singleKey = text.Substring(wordIndex, scanIndex - wordIndex);
                                callback(
                                    Uri.UnescapeDataString(singleKey),
                                    null,
                                    state);

                                break;
                            }

                            // Both key and value available.
                            var key = text.Substring(wordIndex, scanIndex - wordIndex);
                            scanIndex++;
                            var value = text.Substring(scanIndex, delimiterIndex - scanIndex);

                            scanIndex = delimiterIndex;
                            callback(
                                Uri.UnescapeDataString(key),
                                Uri.UnescapeDataString(value),
                                state);
                        }
                        else if (p[scanIndex].Equals('+'))
                            p[scanIndex] = ' ';
                        else
                        {
                            scanIndex++;
                            if (scanIndex == delimiterIndex)
                            {
                                // End of sequence, and no value was detected.
                                // Add sequence as key.
                                var key = text.Substring(wordIndex, scanIndex - wordIndex);
                                callback(
                                    Uri.UnescapeDataString(key),
                                    null,
                                    state);
                            }
                        }
                    }
                }
        }
    }
}