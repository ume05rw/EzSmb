using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EzSmb.Streams.Caches
{
    public class CacheSet : IDisposable
    {
        private bool disposedValue;

        public MemoryStream Cache { get; internal set; }
        public IReadOnlyCollection<Range> Ramainings { get; internal set; }

        public CacheSet()
        {
            this.Cache = new MemoryStream();
            this.Ramainings = Array.Empty<Range>().ToList().AsReadOnly();
        }

        internal void SetRamainings(Range[] ranges)
        {
            this.Ramainings = (ranges ?? Array.Empty<Range>())
                .ToList()
                .AsReadOnly();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        this.Cache?.Close();
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        this.Cache?.Dispose();
                    }
                    catch (Exception)
                    {
                    }

                    this.Cache = null;
                    this.Ramainings = null;
                }

                this.disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
