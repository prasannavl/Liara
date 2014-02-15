// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;

namespace Liara.Logging
{
    public static class LogWriterFactory
    {
        public static ILiaraLogWriter Create()
        {
            if (Environment.UserInteractive)
            {
                return new ConsoleLogWriter();
            }
            // TODO: Return an event log writer by default.
            // TODO: Also write an N-Log Wrapper.
            return new ConsoleLogWriter();
        }

        public static ILiaraLogWriter Create(Type type)
        {
            return (ILiaraLogWriter) Activator.CreateInstance(type);
        }
    }
}