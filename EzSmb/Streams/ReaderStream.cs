using EzSmb.Shareds;
using EzSmb.Shareds.Interfaces;
using EzSmb.Streams.Caches;
using EzSmb.Transports;
using EzSmb.Transports.Shares.Handlers.Enums;
using EzSmb.Transports.Shares.Interfaces;
using SMBLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EzSmb.Streams
{
    /// <summary>
    /// Smb Reader Stream
    /// </summary>
    /// <remarks>
    /// Read Only.
    /// </remarks>
    public class ReaderStream : Stream, IErrorManaged, IDisposable
    {
        private Connection _connection;
        private IShare _share;
        private long _position;
        private long _length;
        private bool _isUseCache;
        private Cache _cache;
        private Locker _locker;


        /// <summary>
        /// Readable stream or not.
        /// </summary>
        public override bool CanRead => true;

        /// <summary>
        /// Seekable stream or not.
        /// </summary>
        public override bool CanSeek => true;

        /// <summary>
        /// Writable stream or not.
        /// </summary>
        public override bool CanWrite => false;

        /// <summary>
        /// Timeout enable or not.
        /// </summary>
        public override bool CanTimeout => true;

        /// <summary>
        /// Stream length.
        /// </summary>
        public override long Length => this._length;

        /// <summary>
        /// Timeout mSec.
        /// </summary>
        public override int ReadTimeout { get; set; }

        /// <summary>
        /// Current position.
        /// </summary>
        public override long Position
        {
            get => this._position;
            set
            {
                this.Seek(value, SeekOrigin.Begin);
            }
        }


        /// <summary>
        /// Source Node Name
        /// </summary>
        public string NodeName { get; private set; }

        /// <summary>
        /// Source Node Full-Path
        /// </summary>
        public string FullPath { get; private set; }

        /// <summary>
        /// Source Node Elements-Path
        /// </summary>
        public string ElementsPath { get; private set; }

        /// <summary>
        /// Cache all responses of in a MemoryStream.
        /// </summary>
        /// <remarks>
        /// Default: false
        ///
        /// It caches the data once retrieved, and returns
        /// the data from the cache when it is retrieved again.
        /// The cache will be clear when it's disposed.
        /// </remarks>
        public bool IsUseCache
        {
            get => this._isUseCache;
            set
            {
                var changed = (this._isUseCache != value);
                this._isUseCache = value;

                if (!changed)
                    return;

                if (this._isUseCache)
                {
                    this._cache = new Cache();
                }
                else
                {
                    this.DisposeCache();
                }
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node"></param>
        internal ReaderStream(Node node) : base()
        {
            this._locker = new Locker();
            this._errors = new List<string>();

            if (node == null)
            {
                this.AddError("Constructor", "Requires node.");

                return;
            }

            this.NodeName = string.Copy(node.Name);
            this.FullPath = string.Copy(node.PathSet.FullPath);

            if (node.Type != NodeType.File)
                this.AddError("Constructor", $"InvalidOperation: NodeTyoe.{node.Type}");
            if (node.Size == null)
                // Shouldn't exist.
                this.AddError("Constructor", "node.Size not setted.");

            if (this.HasError)
                return;

            this._connection = new Connection(node.PathSet, node.ParamSet);
            if (this._connection.HasError)
            {
                this.CopyErrors(this._connection);

                return;
            }

            this._share = this._connection.GetShare();
            if (this._share.HasError)
            {
                this.CopyErrors(this._share);

                return;
            }

            this.ElementsPath = this._share.FormatPath(node.PathSet.ElementsPath);
            this._length = (long)node.Size;
            this._position = 0;
            this._isUseCache = false;
            this.ReadTimeout = 0;
        }

        /// <summary>
        /// Clear caches
        /// </summary>
        public override void Flush()
        {
            this._locker.LockedInvoke(() =>
            {
                this._cache?.Flush();
            });
        }

        /// <summary>
        /// Seek
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException("EzSmb.Streams.ReaderStream");

            return this._locker.LockedInvoke<long>(() =>
            {
                long ordered;
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        ordered = offset;
                        break;
                    case SeekOrigin.End:
                        ordered = this._length + offset;
                        break;
                    case SeekOrigin.Current:
                        ordered = this._position + offset;
                        break;
                    default:
                        throw new ArgumentException($"Unexpected origin: {origin}");
                }

                if (ordered < 0)
                {
                    this._position = 0;
                }
                else if (this._length < ordered)
                {
                    this._position = this._length;
                }
                else
                {
                    this._position = ordered;
                }

                return this._position;
            });
        }


        /// <summary>
        /// Read
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException("EzSmb.Streams.ReaderStream");
            if (this._share == null || !this._share.IsConnected)
                throw new IOException("Not Connected.");
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (buffer.Length < (offset + count))
                throw new ArgumentException("buffer.Length is not enough.");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            var canceller = new CancellationTokenSource();
            if (0 < this.ReadTimeout)
            {
                Task.Delay(this.ReadTimeout)
                    .ContinueWith(t =>
                    {
                        canceller.Cancel();
                    })
                    .ConfigureAwait(false);
            }

            return this._locker.LockedInvoke<int>(() =>
            {
                if (this.IsUseCache)
                {
                    long initialPosition = this._position;

                    var cacheSet = this._cache.GetCacheSet(initialPosition, count);
                    if (cacheSet.Ramainings.Count <= 0)
                    {
                        // All the data had been cached.
                        cacheSet.Cache.ToArray().CopyTo(buffer, offset);
                        this._position = initialPosition + count;

                        return (int)cacheSet.Cache.Length;
                    }

                    var index = 0;
                    foreach (var range in cacheSet.Ramainings)
                    {
                        var partialBuffer = new byte[range.Count];
                        this._position = range.Position;

                        canceller.Token.ThrowIfCancellationRequested();

                        var readed = this.InnerRead(
                            partialBuffer,
                            0,
                            (int)range.Count,
                            canceller.Token
                        );

                        if (
                            (readed < range.Count)
                            && ((range.Position + range.Count) < this.Length)
                        )
                        {
                            throw new IOException("*** File Reading Failed with ReaderStream.IsUseCache = true ***");
                        }

                        cacheSet.Cache.Position = range.Position - initialPosition;
                        cacheSet.Cache.Write(partialBuffer, 0, readed);
                        this._position = range.Position + readed;
                        index++;
                    }

                    cacheSet.Cache.ToArray().CopyTo(buffer, offset);

                    return (int)cacheSet.Cache.Length;
                }
                else
                {
                    return this.InnerRead(buffer, offset, count, canceller.Token);
                }
            });
        }

        private int InnerRead(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancelToken
        )
        {
            if (this.disposedValue)
                throw new ObjectDisposedException("EzSmb.Streams.ReaderStream");
            if (this._share == null || !this._share.IsConnected)
                throw new IOException("Not Connected.");
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (buffer.Length < (offset + count))
                throw new ArgumentException("buffer.Length is not enough.");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            cancelToken.ThrowIfCancellationRequested();

            int readed = 0;
            Exception exception = null;
            using (var hdr = this._share.GetHandler(this.ElementsPath, HandleType.Read, NodeType.File))
            {
                if (!hdr.Succeeded)
                    throw new IOException("File Reading Failed.");

                var initialPosition = this._position;

                while (true)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        exception = new OperationCanceledException(cancelToken);

                        break;
                    }

                    var remainingLength = count - readed;
                    if (remainingLength <= 0)
                        break;

                    var queryLength = (this._share.Store.MaxReadSize < remainingLength)
                        ? (int)this._share.Store.MaxReadSize
                        : remainingLength;

                    var status = this._share.Store.ReadFile(
                        out var data,
                        hdr.Handle,
                        this._position,
                        (int)queryLength
                    );

                    if (cancelToken.IsCancellationRequested)
                    {
                        exception = new OperationCanceledException(cancelToken);

                        break;
                    }

                    if (
                        status != NTStatus.STATUS_SUCCESS
                        && status != NTStatus.STATUS_END_OF_FILE
                    )
                    {
                        exception = new IOException("File Reading Failed. NTStatus: {status}");
                        this.AddError("ReadStream", $"File Reading Failed. NTStatus: {status}");

                        break;
                    }

                    if (status == NTStatus.STATUS_END_OF_FILE || data.Length == 0)
                        break;

                    data.CopyTo(buffer, offset + readed);
                    readed += data.Length;
                    this._position += data.Length;
                }

                if (exception == null && this.IsUseCache)
                    using (var stream = new MemoryStream(buffer.Skip(offset).Take(readed).ToArray()))
                        this._cache.Add(initialPosition, stream);
            }

            if (exception != null)
                throw exception;

            return readed;
        }

        #region "Not Supported"

        /// <summary>
        /// Not Supported.
        /// </summary>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not Supported.
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        #endregion "Not Supported"


        #region "IErrorManaged Implements"

        private List<string> _errors;

        /// <summary>
        /// Error string array
        /// </summary>
        public string[] Errors => this._errors.ToArray();

        /// <summary>
        /// Error flag
        /// </summary>
        public bool HasError => (0 < this._errors.Count);

        /// <summary>
        /// Add error string
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="message"></param>
        protected void AddError(string methodName, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            this._errors.Add($"{DateTime.Now:HH:mm:ss.fff}: [{this.GetType()}.{methodName}] {message}");
        }

        /// <summary>
        /// Add error string
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        protected void AddError(string methodName, string message, Exception ex)
        {
            if (string.IsNullOrEmpty(message) && ex == null)
                return;

            this.AddError(methodName, $"{message}, Exception.Message: {ex.Message}, Exception.StackTrace: {ex.StackTrace}");
        }

        /// <summary>
        /// Add error string from IErrorManaged
        /// </summary>
        /// <param name="errorManaged"></param>
        protected void CopyErrors(IErrorManaged errorManaged)
        {
            if (errorManaged == null || !errorManaged.HasError)
                return;

            var errors = errorManaged.Errors;
            foreach (var message in errors)
                this._errors.Add(message);
        }

        /// <summary>
        /// Clear error strings
        /// </summary>
        public void ClearErrors()
            => this._errors.Clear();


        #endregion "IErrorManaged Implements"

        #region "IDisposable Implements"

        private bool disposedValue;

        private void DisposeCache()
        {
            if (this._cache == null)
                return;

            try
            {
                this._cache.Dispose();
            }
            catch (Exception)
            {
            }

            this._cache = null;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.DisposeCache();
                    this._errors?.Clear();
                    this._share?.Dispose();
                    this._connection?.Dispose();

                    this.NodeName = null;
                    this.FullPath = null;
                    this.ElementsPath = null;

                    this._connection = null;
                    this._share = null;
                    this._position = 0;
                    this._length = 0;
                    this._isUseCache = false;
                    this._cache = null;
                    this._locker = null;
                    this._errors = null;
                }

                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }

        #endregion "IDisposable Implements"
    }
}
