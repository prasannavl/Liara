// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.IO;
using System.Threading.Tasks;
using Liara.Common;
using Liara.Constants;
using Liara.Helpers;

namespace Liara.Formatting
{
    public class UrlEncodedFormFormatter : LiaraFormatter
    {
        private static readonly Action<string, string, object> AppendItemCallback = (name, value, state) =>
        {
            var formCollection = (ILiaraHashTable<string>)state;
            formCollection.AppendValue(name, value);
        };

        public UrlEncodedFormFormatter()
        {
            SupportedMediaTypes.Add(new MediaType(MediaTypeConstants.ApplicationUrlEncodedForm));
        }

        public override bool CanWrite(Type inputObjectType, ILiaraContext context)
        {
            return false;
        }

        public override Task<object> ReadAsync(Type readAsType, Stream inputStream, ILiaraContext context)
        {
            var text = new StreamReader(inputStream).ReadToEnd();
            var collection = ParseForm(text, isCaseSensitive: true);
            return Task.FromResult((object) collection);
        }

        public override Task WriteAsync(object inputObject, Stream targetStream, ILiaraContext context)
        {
            return null;
        }

        public static ILiaraHashTable<string> ParseForm(string text, bool isCaseSensitive = true)
        {
            var collection = new LiaraStringHashTable(isCaseSensitive);
            StringHelpers.ParseUrlEncodedString(text, new[] {'&'}, AppendItemCallback, collection);
            return collection;
        }
    }
}