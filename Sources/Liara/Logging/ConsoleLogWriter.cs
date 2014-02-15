// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Liara.Logging
{
    public sealed class ConsoleLogWriter : ILiaraLogWriter
    {
        private static readonly IndentedConsoleWriter ConsoleWriter = new IndentedConsoleWriter();
        private readonly object lockObj = new object();

        public ConsoleLogWriter()
        {
            IsEnabled = true;
        }

        public int Priority { get; set; }
        public bool IsEnabled { get; set; }

        /// <summary>
        ///     Write to log with the default log name.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="arguments"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Write(string message, params object[] arguments)
        {
            if (!IsEnabled)
                return;
            WriteInternal(GetCallerTypeName(), string.Format(message, arguments));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task WriteAsync(string message, params object[] arguments)
        {
            if (!IsEnabled)
                return;
            var logName = GetCallerTypeName(3);
            await Task.Run(() => WriteInternal(logName, string.Format(message, arguments)));
        }

        /// <summary>
        ///     Write to log with the given log name.
        /// </summary>
        /// <param name="logName">Can be either a string or the object who's type name will be used as the name.</param>
        /// <param name="message">The message string.</param>
        /// <param name="arguments">The message string format arguments.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void WriteTo(string logName, string message, params object[] arguments)
        {
            if (!IsEnabled)
                return;
            WriteInternal(logName, string.Format(message, arguments));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task WriteToAsync(string logName, string message, params object[] arguments)
        {
            if (!IsEnabled)
                return;

            await Task.Run(() => WriteInternal(logName, string.Format(message, arguments)));
        }

        /// <summary>
        ///     Write exception details to log with the default log name.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="throwException">Throws the exception after writing to log.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void WriteException(Exception exception, bool throwException = false)
        {
            if (!IsEnabled)
                return;
            WriteExceptionInternal(GetCallerTypeName(), exception);
            if (throwException)
                throw exception;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task WriteExceptionAsync(Exception exception, bool throwException = false)
        {
            if (!IsEnabled)
                return;
            var logName = GetCallerTypeName(3);
            await Task.Run(() => WriteExceptionInternal(logName, exception));
            if (throwException)
                throw exception;
        }

        /// <summary>
        ///     Write exception details to the log with the given log name.
        /// </summary>
        /// <param name="logName">Can be either a string or the object who's type name will be used as the name.</param>
        /// <param name="exception"></param>
        /// <param name="throwException">Throws the exception after writing to log.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void WriteExceptionTo(string logName, Exception exception, bool throwException = false)
        {
            if (!IsEnabled)
                return;
            WriteExceptionInternal(logName, exception);
            if (throwException)
                throw exception;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task WriteExceptionToAsync(string logName, Exception exception, bool throwException = false)
        {
            if (!IsEnabled)
                return;
            await Task.Run(() => WriteExceptionInternal(logName, exception));
            if (throwException)
                throw exception;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetCallerTypeName(int skipFrames = 1)
        {
            var framesToSkip = skipFrames;
            if (System.Diagnostics.Debugger.IsAttached)
            {
                framesToSkip++;
            }
            var stackFrame = new StackFrame(framesToSkip);
// ReSharper disable once PossibleNullReferenceException
            return stackFrame.GetMethod().DeclaringType.FullName;
        }

        private void WriteExceptionInternal(string logName, Exception exception)
        {
            lock (lockObj)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                WriteExceptionHandler(logName, exception);
                Console.WriteLine("End of exception.");
                Console.ResetColor();
                Console.WriteLine();
            }
        }

        private void WriteCurrentDateTime()
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(DateTime.Now.ToString("s"));
            Console.ForegroundColor = color;
        }

        private void WriteExceptionHandler(string logName, Exception exception)
        {
            WriteCurrentDateTime();
            Console.WriteLine(" - {0} - Exception:", logName);
            ConsoleWriter.Indent++;

            if (exception.GetType() == typeof (AggregateException))
            {
                Console.WriteLine(" ExceptionType: {0}", exception.GetType().Name);
                Console.WriteLine();
                Console.WriteLine(" Inner Exceptions:");
                Console.WriteLine();
                ConsoleWriter.Indent++;
                foreach (var ex in ((AggregateException) exception).InnerExceptions)
                {
                    WriteExceptionHandler(logName, ex);
                }
                ConsoleWriter.Indent--;
            }

            PrintExceptionExplanation(exception);
            ConsoleWriter.Indent--;
        }

        private void PrintExceptionExplanation(Exception exception)
        {
            if (exception.GetType() != typeof (AggregateException))
            {
                Console.WriteLine("ExceptionType: {0}", exception.GetType().Name);
                Console.WriteLine("Message: {0}", exception.Message);
                Console.WriteLine("Source: {0}", exception.Source);
                Console.WriteLine("TargetSite: {0}", exception.TargetSite);
                Console.WriteLine("StackTrace: ");
                Console.WriteLine(exception.StackTrace);
            }
        }

        private void WriteInternal(string logName, string message)
        {
            lock (lockObj)
            {
                WriteCurrentDateTime();
                Console.WriteLine(" - {0}: {1}",
                    logName, message);
            }
        }

        private class IndentedConsoleWriter : TextWriter
        {
            private readonly TextWriter sysConsole;
            private bool doIndent;

            public IndentedConsoleWriter()
            {
                sysConsole = Console.Out;
                Console.SetOut(this);
            }

            public int Indent { get; set; }

            public override System.Text.Encoding Encoding
            {
                get { return sysConsole.Encoding; }
            }

            public override void Write(char ch)
            {
                if (doIndent)
                {
                    doIndent = false;
                    for (int ix = 0; ix < Indent; ++ix) sysConsole.Write("  ");
                }
                sysConsole.Write(ch);
                if (ch == '\n') doIndent = true;
            }
        }
    }
}