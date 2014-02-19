// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 1:44 AM 17-02-2014

namespace Liara.Formatting
{
    public interface ILiaraViewProvider
    {
        string GetView(ILiaraContext context);
        string GetInternalView(ILiaraContext context);
    }
}