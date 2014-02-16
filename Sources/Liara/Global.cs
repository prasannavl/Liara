// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 3:56 PM 15-02-2014

using Liara.Logging;

namespace Liara
{
    public class Global
    {
        public static ILiaraLogWriter FrameworkLogger = LogWriterFactory.Create();

        static Global()
        {
            FrameworkLogger.IsEnabled = true;
        }
    }
}