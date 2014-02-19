// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 9:28 PM 17-02-2014

namespace Liara.Formatting
{
    public interface ILiaraViewTemplate
    {
        string Name { get; set; }
        object Template { get; set; }
        bool IsInternalTemplate { get; set; }
    }
}