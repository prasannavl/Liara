// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 7:05 AM 19-02-2014

using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Liara.Common
{
    public class LiaraStringHashTable : LiaraHashTable<string>
    {
        public LiaraStringHashTable(bool isCaseSensitive = true) : base(isCaseSensitive)
        {
        }

        public LiaraStringHashTable(IDictionary<string, string[]> hashTable)
            : base(hashTable)
        {
        }

        public override string Get(string key)
        {
            string[] values;
            return TryGetValue(key, out values) ? String.Join(",", values) : null;
        }

        /// <summary>
        ///     Provides the implementation for operations that set member values. Classes derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for
        ///     operations such as setting a value for a property.
        /// </summary>
        /// <returns>
        ///     true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of
        ///     the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        /// <param name="binder">
        ///     Provides information about the object that called the dynamic operation. The binder.Name property
        ///     provides the name of the member to which the value is being assigned. For example, for the statement
        ///     sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase
        ///     property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="value">
        ///     The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where
        ///     sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the
        ///     <paramref name="value" /> is "Test".
        /// </param>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Set(binder.Name, value.ToString());
            return true;
        }

        /// <summary>
        ///     Provides the implementation for operations that get member values. Classes derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for
        ///     operations such as getting a value for a property.
        /// </summary>
        /// <returns>
        ///     true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of
        ///     the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        /// <param name="binder">
        ///     Provides information about the object that called the dynamic operation. The binder.Name property
        ///     provides the name of the member on which the dynamic operation is performed. For example, for the
        ///     Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived
        ///     from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The
        ///     binder.IgnoreCase property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="result">
        ///     The result of the get operation. For example, if the method is called for a property, you can
        ///     assign the property value to <paramref name="result" />.
        /// </param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string[] typeBuffer;
            var hasValue = TryGetValue(binder.Name, out typeBuffer);
            if (hasValue)
            {
                result = typeBuffer;
                return true;
            }

            result = null;
            return false;
        }
    }
}