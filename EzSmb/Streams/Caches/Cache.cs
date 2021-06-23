using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EzSmb.Streams.Caches
{
    internal class Cache : IDisposable
    {
        private List<Range> _ranges;
        private MemoryStream _cacheStream;
        private bool disposedValue;

        public Cache()
        {
            this._ranges = new List<Range>();

            this.Flush();
        }

        /// <summary>
        /// for test
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<Range> GetRanges()
            => this._ranges.ToList().AsReadOnly();

        public void Flush()
        {
            if (this._cacheStream != null)
            {
                try
                {
                    this._cacheStream.Close();
                }
                catch (Exception)
                {
                }
                try
                {
                    this._cacheStream.Dispose();
                }
                catch (Exception)
                {
                }
            }
            this._cacheStream = new MemoryStream();

            this._ranges.Clear();
        }

        /// <summary>
        /// Add Range
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="memory"></param>
        public void Add(long offset, MemoryStream memory)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException("EzSmb.Streams.Caches.FileCache");
            if (memory == null)
                throw new ArgumentException("Required memory");

            // ファイルStreamの長さが足りないとき、拡張する。
            var fullLength = offset + memory.Length;
            if (this._cacheStream.Length < fullLength)
                this._cacheStream.SetLength(fullLength);

            this._cacheStream.Position = offset;
            memory.Position = 0;
            memory.CopyTo(this._cacheStream);

            var end = offset + memory.Length - 1;
            this._ranges.Add(new Range(offset, end));

            this.Merge();
        }

        private void Merge()
        {
            if (this.disposedValue)
                throw new ObjectDisposedException("EzSmb.Streams.Caches.FileCache");

            if (this._ranges.Count <= 1)
                return;

            // BeginのAsc順にソート
            this._ranges = this._ranges
                .OrderBy(e => e.Start)
                .ToList();

            var deletes = new List<Range>();
            Range target = this._ranges[0];
            for (var i = 1; i < this._ranges.Count; i++)
            {
                var current = this._ranges[i];

                if (current.Start <= (target.End + 1))
                {
                    // 直前要素の後半と現要素の前半が重複している。
                    // target範囲にcurrentを含める
                    if (target.End < current.End)
                        target.End = current.End;

                    // currentは削除対象にする
                    deletes.Add(current);

                    // targetを維持して次へ
                    continue;
                }

                // 範囲の重複が無い
                //  -> 比較対象を更新して次へ。
                target = current;
            }

            foreach (var delete in deletes)
                this._ranges.Remove(delete);
        }

        /// <summary>
        /// Get Range-Array to be query.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Range[] GetRamainings(long offset, long count)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException("EzSmb.Streams.Caches.FileCache");

            var end = offset + count - 1;
            var argRange = new Range(offset, end);
            var result = new List<Range>();
            var pos = argRange.Start;

            // this._ranges はメソッド開始時点でAscソート済み。
            foreach (var exists in this._ranges)
            {
                if (argRange.End <= pos)
                    // 要求範囲の走査が終わった
                    break;

                if (exists.End < pos)
                    // 要求範囲はexists範囲の後にある
                    //  -> 既読範囲なし、スキップ
                    continue;

                if (
                    (exists.Start <= argRange.Start && argRange.Start <= exists.End)
                    && (exists.Start <= argRange.End && argRange.End <= exists.End)
                )
                {
                    // 要求範囲はexistsに内包されている
                    //  -> 全て既読範囲、空配列を返して即時終了。
                    return Array.Empty<Range>();
                }

                if (pos < exists.Start)
                {
                    // 要求範囲はexists開始点より前方に残っている
                    //  -> existsに含まれない前方の要求範囲を戻り値に追加
                    //     既読範囲をスキップ
                    var endPoint = (argRange.End < (exists.Start - 1))
                        ? argRange.End
                        : (exists.Start - 1);
                    result.Add(new Range(pos, endPoint));
                    pos = exists.End + 1;

                    continue;
                }

                if (pos <= exists.End)
                {
                    // 要求範囲はexists終了点より前にある
                    //  -> 既読範囲をスキップ
                    pos = exists.End + 1;
                }
            }

            if (pos < argRange.End)
                // 全既読範囲より後方に要求範囲が残っている
                //  -> 残りを戻り値に追加
                result.Add(new Range(pos, argRange.End));

            return result.ToArray();
        }

        public CacheSet GetCacheSet(
            long offset,
            long count
        )
        {
            if (this.disposedValue)
                throw new ObjectDisposedException("EzSmb.Streams.Caches.FileCache");

            var result = new CacheSet();
            result.SetRamainings(this.GetRamainings(offset, count));

            if (this._cacheStream.Length < offset)
            {
                // キャッシュ長が指定位置に満たないとき
                //  -> MemoryStreamは空で、Range[]は判定結果を返す。
                return result;
            }

            // 取得長を取得する
            var orderedFullLength = offset + count;
            var remainingFullLength = orderedFullLength - this._cacheStream.Length;
            var reqFullLength = (remainingFullLength < 0)
                ? count
                : (this._cacheStream.Length - offset);

            var maxReqLength = 262144; // 256KB
            var buffer = new byte[maxReqLength];
            long completed = 0;

            // 指定長(256KB)ずつ取得するループ
            result.Cache.Position = 0;
            while (completed < reqFullLength)
            {
                var reamainingLength = reqFullLength - completed;
                var reqLength = (reamainingLength < maxReqLength)
                    ? (int)reamainingLength
                    : maxReqLength;

                // 位置は取得直前に毎回セットする
                this._cacheStream.Position = offset + completed;
                var readLength = this._cacheStream.Read(buffer, 0, reqLength);

                completed += readLength;
                result.Cache.Write(buffer, 0, readLength);
            }

            return result;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        this._cacheStream.Close();
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        this._cacheStream.Dispose();
                    }
                    catch (Exception)
                    {
                    }

                    this._ranges?.Clear();
                    this._ranges = null;
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
