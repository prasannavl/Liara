// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Runtime;

namespace Liara.Internal
{
    // ReSharper disable once InconsistentNaming
    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    internal sealed class __DynamicallyInvokableAttribute : Attribute
    {
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public __DynamicallyInvokableAttribute()
        {
        }
    }
}