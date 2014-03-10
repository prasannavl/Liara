// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Liara.ResponseProcessing
{
    public class LiaraResponse : ILiaraResponse
    {
        private readonly ILiaraContext context;
        private LiaraResponseCookieCollection cookies;
        private bool isSynced;

        public LiaraResponse(ILiaraContext context)
        {
            this.context = context;
            Format = new LiaraResponseFormatProvider(context);
        }

        public LiaraHttpStatus Status
        {
            get
            {
                return new LiaraHttpStatus(context.Environment.ResponseStatusCode,
                    context.Environment.ResponseStatusDescription);
            }

            set
            {
                context.Environment.ResponseStatusCode = value.Code;
                context.Environment.ResponseStatusDescription = value.Description;
            }
        }

        public LiaraResponseHeaderCollection Headers
        {
            get { return new LiaraResponseHeaderCollection(context); }
        }

        public LiaraResponseCookieCollection Cookies
        {
            get { return cookies ?? (cookies = new LiaraResponseCookieCollection(context)); }
        }

        public object Content { get; set; }

        public LiaraResponseFormatProvider Format { get; set; }

        public void Synchronize()
        {
            context.Engine.Configuration.ResponseSynchronizer.Synchronize(context);
            isSynced = true;
        }

        /// <summary>
        ///     Writes the given text to the response body stream using UTF-8.
        /// </summary>
        /// <param name="text">The response data.</param>
        public void Write(string text)
        {
            Write(Encoding.UTF8.GetBytes(text));
        }

        /// <summary>
        ///     Writes the given bytes to the response body stream.
        /// </summary>
        /// <param name="data">The response data.</param>
        public void Write(byte[] data)
        {
            Write(data, 0, data == null ? 0 : data.Length);
        }

        /// <summary>
        ///     Writes the given bytes to the response body stream.
        /// </summary>
        /// <param name="data">The response data.</param>
        /// <param name="offset">
        ///     The zero-based byte offset in the <paramref name="data" /> parameter at which to begin copying
        ///     bytes.
        /// </param>
        /// <param name="count">The number of bytes to write.</param>
        public void Write(byte[] data, int offset, int count)
        {
            if (!isSynced) Synchronize();
            context.Environment.ResponseBody.Write(data, offset, count);
        }

        /// <summary>
        ///     Asynchronously writes the given text to the response body stream using UTF-8.
        /// </summary>
        /// <param name="text">The response data.</param>
        /// <returns>A Task tracking the state of the write operation.</returns>
        public Task WriteAsync(string text)
        {
            return WriteAsync(text, CancellationToken.None);
        }

        /// <summary>
        ///     Asynchronously writes the given text to the response body stream using UTF-8.
        /// </summary>
        /// <param name="text">The response data.</param>
        /// <param name="token">A token used to indicate cancellation.</param>
        /// <returns>A Task tracking the state of the write operation.</returns>
        public Task WriteAsync(string text, CancellationToken token)
        {
            return WriteAsync(Encoding.UTF8.GetBytes(text), token);
        }

        /// <summary>
        ///     Asynchronously writes the given bytes to the response body stream.
        /// </summary>
        /// <param name="data">The response data.</param>
        /// <returns>A Task tracking the state of the write operation.</returns>
        public Task WriteAsync(byte[] data)
        {
            return WriteAsync(data, CancellationToken.None);
        }

        /// <summary>
        ///     Asynchronously writes the given bytes to the response body stream.
        /// </summary>
        /// <param name="data">The response data.</param>
        /// <param name="token">A token used to indicate cancellation.</param>
        /// <returns>A Task tracking the state of the write operation.</returns>
        public Task WriteAsync(byte[] data, CancellationToken token)
        {
            return WriteAsync(data, 0, data == null ? 0 : data.Length, token);
        }

        /// <summary>
        ///     Asynchronously writes the given bytes to the response body stream.
        /// </summary>
        /// <param name="data">The response data.</param>
        /// <param name="offset">
        ///     The zero-based byte offset in the <paramref name="data" /> parameter at which to begin copying
        ///     bytes.
        /// </param>
        /// <param name="count">The number of bytes to write.</param>
        /// <param name="token">A token used to indicate cancellation.</param>
        /// <returns>A Task tracking the state of the write operation.</returns>
        public Task WriteAsync(byte[] data, int offset, int count, CancellationToken token)
        {
            if (!isSynced) Synchronize();
            return context.Environment.ResponseBody.WriteAsync(data, offset, count, token);
        }
    }
}