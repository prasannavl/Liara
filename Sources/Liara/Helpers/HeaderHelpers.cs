// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Liara.Common;

namespace Liara.Helpers
{
    public class HeaderHelpers
    {
        public static unsafe List<List<string>> ParseHttpHeaderValue(string headerString)
        {
            if (headerString == null)
            {
                return new List<List<string>>();
            }
            var scanIndex = 0;
            var startIndex = scanIndex;
            var isScanningInQuotedZone = false;

            var segments = new List<List<string>>();
            var innerList = new List<string>();

            fixed (char* p = headerString)
                while (scanIndex < headerString.Length)
                {
                    if (p[scanIndex] == '\"')
                    {
                        isScanningInQuotedZone = !isScanningInQuotedZone;
                    }
                    if (p[scanIndex] == ' ')
                    {
                        if (!isScanningInQuotedZone) p[scanIndex] = '\r';
                        scanIndex++;
                        continue;
                    }
                    if (p[scanIndex] == ';' && !isScanningInQuotedZone)
                    {
                        innerList.Add(headerString.Substring(startIndex, scanIndex - startIndex)
                            .Replace("\r", string.Empty));
                        startIndex = scanIndex + 1;
                    }
                    if (p[scanIndex] == ',' && !isScanningInQuotedZone)
                    {
                        innerList.Add(headerString.Substring(startIndex, scanIndex - startIndex)
                            .Replace("\r", string.Empty));
                        segments.Add(innerList);
                        startIndex = scanIndex + 1;
                        innerList = new List<string>();
                    }

                    scanIndex++;
                }

            var lastIndex = headerString.Length;

            if (startIndex < lastIndex)
            {
                innerList.Add(headerString.Substring(startIndex, lastIndex - startIndex).Replace("\r", string.Empty));
                segments.Add(innerList);
            }

            return segments;
        }


        public static IEnumerable<LiaraQValuedHeader> ParseQValuedHeaders(string headerString)
        {
            var parsedSegments = ParseHttpHeaderValue(headerString);
            foreach (var segment in parsedSegments)
            {
                var qValuedHeader = new LiaraQValuedHeader();
                var qAdded = false;
                foreach (var part in segment)
                {
                    if (!qAdded && part.StartsWith("q="))
                    {
                        float q;
                        var parsed = float.TryParse(part.Substring(2), out q);
                        qValuedHeader.QValue = parsed ? q : 1;
                        qAdded = true;
                    }
                    else if (qValuedHeader.Value != null)
                    {
                        qValuedHeader.Value = part;
                    }
                }

                yield return qValuedHeader;
            }
        }

        public static IEnumerable<LiaraQValuedMultiItemHeader> ParseQValuedMultiItemHeaders(string headerString)
        {
            var parsedSegments = ParseHttpHeaderValue(headerString);
            foreach (var segment in parsedSegments)
            {
                var qValuedHeader = new LiaraQValuedMultiItemHeader();
                var qAdded = false;
                foreach (var part in segment)
                {
                    if (!qAdded && part.StartsWith("q="))
                    {
                        float q;
                        var parsed = float.TryParse(part.Substring(2), out q);
                        qValuedHeader.QValue = parsed ? q : 1;
                        qAdded = true;
                    }
                    else
                    {
                        qValuedHeader.Values.Add(part);
                    }
                }

                yield return qValuedHeader;
            }
        }

        public static IEnumerable<T> GetPrioritizedQValuedHeaders<T>(IEnumerable<T> qValuedHeaders)
            where T : IQValuedItem
        {
            return qValuedHeaders.OrderByDescending(q => q.QValue);
        }

        public static IEnumerable<QValuedMediaType> GetQValuedMediaTypeHeaders(string mediaTypeString)
        {
            var parsedSegments = ParseHttpHeaderValue(mediaTypeString);
            foreach (List<string> segment in parsedSegments)
            {
                var mediaType = new QValuedMediaType();
                var qAdded = false;
                for (int i = 0; i < segment.Count; i++)
                {
                    if (i == 0)
                    {
                        var split = segment[i].Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                        if (split.Length > 0)
                            mediaType.Group = split[0];
                        if (split.Length > 1)
                            mediaType.Type = split[1];
                    }
                    else
                    {
                        var part = segment[i];
                        if (!qAdded && part.StartsWith("q="))
                        {
                            float q;
                            var parsed = float.TryParse(part.Substring(2), out q);
                            mediaType.QValue = parsed ? q : 1;
                            qAdded = true;
                        }
                        else
                        {
                            mediaType.ExtendedTokens.Add(part);
                        }
                    }
                }

                if (mediaType.QValue == 0) mediaType.QValue = 1;

                yield return mediaType;
            }
        }

        public static IEnumerable<QValuedMediaType> GetPrioritizedQValuedMediaTypes(IEnumerable<QValuedMediaType> types)
        {
            return
                types.OrderByDescending(m => m.QValue)
                    .ThenByDescending(m => m.ExtendedTokens.Any())
                    // Bring the "*" type strings to the bottom.
                    .ThenByDescending(m => m.Type);
        }

        public static IEnumerable<Encoding> GetCharsetAcceptedEncodings(string charsetHeaderString)
        {
            var encodings = GetPrioritizedQValuedHeaders(ParseQValuedHeaders(charsetHeaderString));
            foreach (var liaraQValuedHeader in encodings)
            {
                Encoding encoding = null;
                try
                {
                    encoding = Encoding.GetEncoding(liaraQValuedHeader.Value);
                }
                catch
                {
                }
                if (encoding != null)
                    yield return encoding;
            }
        }

        public static IEnumerable<CultureInfo> GetAcceptedCultures(string languageHeaderString)
        {
            var cultures = GetPrioritizedQValuedHeaders(ParseQValuedHeaders(languageHeaderString));
            foreach (var liaraQValuedHeader in cultures)
            {
                CultureInfo cInfo = null;
                try
                {
                    cInfo = CultureInfo.GetCultureInfo(liaraQValuedHeader.Value);
                }
                catch
                {
                }
                if (cInfo != null)
                    yield return cInfo;
            }
        }

        public static void GetContentMediaTypeAndCharset(string mediaTypeHeaderString, out MediaType mediaType,
            out Encoding charset)
        {
            var resultLists = ParseHttpHeaderValue(mediaTypeHeaderString);

            mediaType = null;
            charset = null;

            foreach (var item in resultLists.SelectMany(list => list))
            {
                if (mediaType == null)
                {
                    var split = item.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length == 2)
                        mediaType = new MediaType(split[0], split[1]);
                }
                else if (charset == null && item.StartsWith("charset="))
                {
                    Encoding encoding = null;
                    try
                    {
                        encoding = Encoding.GetEncoding(item.Substring(8));
                    }
                    catch
                    {
                    }
                    if (encoding != null)
                    {
                        charset = encoding;
                    }
                }
            }
        }
    }
}