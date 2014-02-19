// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Threading.Tasks;
using Liara.Common;

namespace Liara.Logging
{
    public interface ILiaraLogWriter : ILiaraPrioritizedService
    {
        bool IsEnabled { get; set; }

        /// <summary>
        ///     Write to log with the default log name.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="arguments"></param>
        void Write(string message, params object[] arguments);

        Task WriteAsync(string message, params object[] arguments);

        /// <summary>
        ///     Write to log with the given log name.
        /// </summary>
        /// <param name="logName">Can be either a string or the object who's type name will be used as the name.</param>
        /// <param name="message">The message string.</param>
        /// <param name="arguments">The message string format arguments.</param>
        void WriteTo(string logName, string message, params object[] arguments);

        Task WriteToAsync(string logName, string message, params object[] arguments);


        /// <summary>
        ///     Write exception details to log with the default log name.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="throwException">Throws the exception after writing to log.</param>
        void WriteException(Exception exception, bool throwException = false);

        Task WriteExceptionAsync(Exception exception, bool throwException = false);


        /// <summary>
        ///     Write exception details to the log with the given log name.
        /// </summary>
        /// <param name="logName">Can be either a string or the object who's type name will be used as the name.</param>
        /// <param name="exception"></param>
        /// <param name="throwException">Throws the exception after writing to log.</param>
        void WriteExceptionTo(string logName, Exception exception, bool throwException = false);

        Task WriteExceptionToAsync(string logName, Exception exception, bool throwException = false);
    }
}