// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System.Threading;
using System.Threading.Tasks;

namespace Liara.ResponseProcessing
{
    public interface ILiaraResponse
    {
        LiaraHttpStatus Status { get; set; }
        LiaraResponseHeaderCollection Headers { get; }
        LiaraResponseCookieCollection Cookies { get; }
        object Content { get; set; }
        LiaraResponseFormatProvider Format { get; set; }
        void Synchronize();

        /// <summary>
        ///     Writes the given text to the response body stream using UTF-8.
        /// </summary>
        /// <param name="text">The response data.</param>
        void Write(string text);

        /// <summary>
        ///     Writes the given bytes to the response body stream.
        /// </summary>
        /// <param name="data">The response data.</param>
        void Write(byte[] data);

        /// <summary>
        ///     Writes the given bytes to the response body stream.
        /// </summary>
        /// <param name="data">The response data.</param>
        /// <param name="offset">
        ///     The zero-based byte offset in the <paramref name="data" /> parameter at which to begin copying
        ///     bytes.
        /// </param>
        /// <param name="count">The number of bytes to write.</param>
        void Write(byte[] data, int offset, int count);

        /// <summary>
        ///     Asynchronously writes the given text to the response body stream using UTF-8.
        /// </summary>
        /// <param name="text">The response data.</param>
        /// <returns>A Task tracking the state of the write operation.</returns>
        Task WriteAsync(string text);

        /// <summary>
        ///     Asynchronously writes the given text to the response body stream using UTF-8.
        /// </summary>
        /// <param name="text">The response data.</param>
        /// <param name="token">A token used to indicate cancellation.</param>
        /// <returns>A Task tracking the state of the write operation.</returns>
        Task WriteAsync(string text, CancellationToken token);

        /// <summary>
        ///     Asynchronously writes the given bytes to the response body stream.
        /// </summary>
        /// <param name="data">The response data.</param>
        /// <returns>A Task tracking the state of the write operation.</returns>
        Task WriteAsync(byte[] data);

        /// <summary>
        ///     Asynchronously writes the given bytes to the response body stream.
        /// </summary>
        /// <param name="data">The response data.</param>
        /// <param name="token">A token used to indicate cancellation.</param>
        /// <returns>A Task tracking the state of the write operation.</returns>
        Task WriteAsync(byte[] data, CancellationToken token);

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
        Task WriteAsync(byte[] data, int offset, int count, CancellationToken token);
    }
}