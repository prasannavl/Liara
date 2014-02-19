// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 9:28 PM 17-02-2014

namespace Liara.Formatting
{
    public class LiaraViewTemplate : ILiaraViewTemplate
    {
        public LiaraViewTemplate(string name, object template, bool isInternalTemplate = false)
        {
            Name = name;
            Template = template;
            IsInternalTemplate = isInternalTemplate;
        }

        public string Name { get; set; }
        public object Template { get; set; }
        public bool IsInternalTemplate { get; set; }
    }
}