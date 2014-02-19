// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liara.Constants;

namespace Liara.Logging
{
    public sealed class ConsoleLogWriter : ILiaraLogWriter
    {
        private static readonly IndentedConsoleWriter ConsoleWriter = new IndentedConsoleWriter();
        private static readonly object ConsoleLockObj = new object();
        private Subject<LogMessage> messagePump;
        private int priority = LiaraServiceConstants.OrderDefault;

        public ConsoleLogWriter()
        {
            IsEnabled = true;
            SetupListener();
        }

        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

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


        private void WriteSingleLine(LogMessage message)
        {
            WriteDateTime(message.TimeStamp);
            Console.WriteLine(" - {0}: {1}", message.LogName, message.Message);
        }

        private void WriteMultiLine(IGrouping<string, LogMessage> items)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(items.Key + ":");
            Console.ResetColor();
            Console.WriteLine();
            foreach (var message in items)
            {
                if (!string.IsNullOrWhiteSpace(message.Message))
                    Console.WriteLine(message.Message);
            }
        }

        private void WriteDateTime(DateTime value, string prefix = null, string suffix = null, string stringFormat = "s")
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(prefix + value.ToString(stringFormat) + suffix);
            Console.ResetColor();
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
            lock (ConsoleLockObj)
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
            messagePump.OnNext(new LogMessage {LogName = logName, Message = message, TimeStamp = DateTime.Now});
        }

        private void MessageReceivedAction(IList<LogMessage> events)
        {
            var count = events.Count;
            if (count < 1) return;

            lock (ConsoleLockObj)
            {
                if (count > 1)
                {
                    Console.WriteLine();
                    WriteDateTime(events.First().TimeStamp, suffix: " : \r\n");

                    var category = events.GroupBy(e => e.LogName);

                    foreach (var items in category)
                    {
                        if (items.Count() > 1)
                        {
                            WriteMultiLine(items);
                        }
                        else
                        {
                            Console.WriteLine();
                            var evt = items.First();
                            WriteSingleLine(evt);
                        }
                    }
                    Console.WriteLine();
                }
                else if (count == 1)
                {
                    var evt = events.First();
                    WriteSingleLine(evt);
                }
            }
        }

        private void SetupListener()
        {
            messagePump = new Subject<LogMessage>();
            messagePump.Buffer(TimeSpan.FromSeconds(1)).Subscribe(MessageReceivedAction);
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

        public class LogMessage
        {
            public DateTime TimeStamp { get; set; }
            public string LogName { get; set; }
            public string Message { get; set; }
        }
    }
}