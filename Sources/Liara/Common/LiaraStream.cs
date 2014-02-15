// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Liara.Common
{
    public class LiaraStream : Stream
    {
        private readonly Stream stream;
        private bool disposed;
        private volatile bool isBufferFilled;

        public LiaraStream()
        {
            stream = new MemoryStream();
        }

        public LiaraStream(Stream stream, bool buffered = false)
        {
            if (buffered)
            {
                this.stream = new MemoryStream();

                // Note: Async call not awaited. The stream is filled parallely, as we continue with
                // the context. IsStreamFilled acts an indicator for completion.

                BufferFillTask = CopyStream(stream);

                IsBuffered = true;
            }
            else
            {
                this.stream = stream;
            }
        }

        public bool IsBuffered { get; set; }

        public bool IsBufferFilled
        {
            get { return isBufferFilled; }
            set { isBufferFilled = value; }
        }

        public Task BufferFillTask { get; set; }

        public override bool CanRead
        {
            get { return stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return stream.CanWrite; }
        }

        public override long Length
        {
            get { return stream.Length; }
        }

        public override long Position
        {
            get { return stream.Position; }
            set { stream.Position = value; }
        }


        public override bool CanTimeout
        {
            get { return stream.CanTimeout; }
        }

        public override int ReadTimeout
        {
            get { return stream.ReadTimeout; }
            set { stream.ReadTimeout = value; }
        }

        public override int WriteTimeout
        {
            get { return stream.WriteTimeout; }
            set { stream.WriteTimeout = value; }
        }

        private async Task CopyStream(Stream sourceStream)
        {
            await sourceStream.CopyToAsync(stream);
            IsBufferFilled = true;
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }

        public override void Close()
        {
            stream.Close();
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return stream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override bool Equals(object obj)
        {
            return stream.Equals(obj);
        }

        public override int GetHashCode()
        {
            return stream.GetHashCode();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return stream.FlushAsync(cancellationToken);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return stream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override string ToString()
        {
            return stream.ToString();
        }

        public override int ReadByte()
        {
            return stream.ReadByte();
        }

        public override void WriteByte(byte value)
        {
            stream.WriteByte(value);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return stream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback,
            object state)
        {
            return stream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback,
            object state)
        {
            return stream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return stream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            stream.EndWrite(asyncResult);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposed)
            {
                disposed = true;
                stream.Dispose();
            }
        }
    }
}