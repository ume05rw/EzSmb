using EzSmb.Streams.Caches;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace EzSmbTest.Streams.Caches
{
    public class FileCacheTest
    {
        [Fact]
        public void WriteReadTest()
        {
            using (var set = new FileCache())
            {
                var stream1 = new MemoryStream(Encoding.UTF8.GetBytes("hello?"));
                var stream2 = new MemoryStream(Encoding.UTF8.GetBytes("are you ok?"));
                var stream3 = new MemoryStream(Encoding.UTF8.GetBytes("write and read!"));

                set.Add(0, stream1);
                set.Add(stream1.Length, stream2);
                set.Add(stream1.Length + stream2.Length, stream3);
                var ranges = set.GetRanges();
                Assert.Single(ranges);

                var fullLength = stream1.Length + stream2.Length + stream3.Length;
                var cacheSet = set.GetCacheSet(0, fullLength);

                Assert.NotNull(cacheSet.Cache);
                Assert.Empty(cacheSet.Ramainings);
                Assert.Equal(fullLength, cacheSet.Cache.Length);
                var text = Encoding.UTF8.GetString(cacheSet.Cache.ToArray());
                Assert.Equal("hello?are you ok?write and read!", text);
            }

            using (var set = new FileCache())
            {
                var stream1 = new MemoryStream(Encoding.UTF8.GetBytes("hello?"));
                var stream2 = new MemoryStream(Encoding.UTF8.GetBytes("are you ok?"));
                var stream3 = new MemoryStream(Encoding.UTF8.GetBytes("write and read!"));

                set.Add(0, stream1);
                set.Add(stream1.Length + 10, stream2);
                set.Add(stream1.Length + 10 + stream2.Length + 20, stream3);
                var ranges = set.GetRanges();
                Assert.Equal(3, ranges.Count);

                var fullLength = stream1.Length + 10 + stream2.Length + 20 + stream3.Length;
                var cacheSet = set.GetCacheSet(0, fullLength);

                Assert.NotNull(cacheSet.Cache);
                Assert.Equal(2, cacheSet.Ramainings.Count);

                var rems = cacheSet.Ramainings.ToArray();
                Assert.Equal(stream1.Length, rems[0].Position);
                Assert.Equal(10, rems[0].Count);
                Assert.Equal(stream1.Length + 10 + stream2.Length, rems[1].Position);
                Assert.Equal(20, rems[1].Count);

                Assert.Equal(fullLength, cacheSet.Cache.Length);
                var cacheBytes = cacheSet.Cache.ToArray();

                var st1Bytes = cacheBytes
                    .Take((int)stream1.Length)
                    .ToArray();
                Assert.Equal("hello?", Encoding.UTF8.GetString(st1Bytes));

                var st2Bytes = cacheBytes
                    .Skip((int)stream1.Length + 10)
                    .Take((int)stream2.Length)
                    .ToArray();
                Assert.Equal("are you ok?", Encoding.UTF8.GetString(st2Bytes));

                var st3Bytes = cacheBytes
                    .Skip((int)stream1.Length + 10 + (int)stream2.Length + 20)
                    .Take((int)stream3.Length)
                    .ToArray();
                Assert.Equal("write and read!", Encoding.UTF8.GetString(st3Bytes));
            }
        }


        [Fact]
        public void MergeNotMergedTest()
        {
            using (var set = new FileCache())
            {
                set.Add(10, new MemoryStream(new byte[11]));
                set.Add(30, new MemoryStream(new byte[11]));
                set.Add(50, new MemoryStream(new byte[11]));
                var ranges = set.GetRanges();
                Assert.Equal(3, ranges.Count);
                var beginBefore = long.MinValue;
                var endBefore = long.MinValue;
                foreach (var rng in ranges)
                {
                    Assert.True(beginBefore < rng.Start);
                    Assert.True(endBefore < rng.End);
                    beginBefore = rng.Start;
                    endBefore = rng.End;
                }
            }

            using (var set = new FileCache())
            {
                set.Add(50, new MemoryStream(new byte[11]));
                set.Add(30, new MemoryStream(new byte[11]));
                set.Add(10, new MemoryStream(new byte[11]));
                var ranges = set.GetRanges();
                Assert.Equal(3, ranges.Count);
                var beginBefore = long.MinValue;
                var endBefore = long.MinValue;
                foreach (var rng in ranges)
                {
                    Assert.True(beginBefore < rng.Start);
                    Assert.True(endBefore < rng.End);
                    beginBefore = rng.Start;
                    endBefore = rng.End;
                }
            }

            using (var set = new FileCache())
            {
                set.Add(30, new MemoryStream(new byte[11]));
                set.Add(50, new MemoryStream(new byte[11]));
                set.Add(10, new MemoryStream(new byte[11]));
                var ranges = set.GetRanges();
                Assert.Equal(3, ranges.Count);
                var beginBefore = long.MinValue;
                var endBefore = long.MinValue;
                foreach (var rng in ranges)
                {
                    Assert.True(beginBefore < rng.Start);
                    Assert.True(endBefore < rng.End);
                    beginBefore = rng.Start;
                    endBefore = rng.End;
                }
            }
        }

        [Fact]
        public void MergeToOneTest()
        {
            using (var set = new FileCache())
            {
                set.Add(10, new MemoryStream(new byte[11]));
                set.Add(15, new MemoryStream(new byte[16]));
                set.Add(25, new MemoryStream(new byte[16]));
                var ranges = set.GetRanges();
                Assert.Single(ranges);
                Assert.Equal(10, ranges.First().Start);
                Assert.Equal(40, ranges.First().End);
                Assert.Equal(31, ranges.First().Count);
            }

            using (var set = new FileCache())
            {
                set.Add(25, new MemoryStream(new byte[16]));
                set.Add(15, new MemoryStream(new byte[16]));
                set.Add(10, new MemoryStream(new byte[11]));
                var ranges = set.GetRanges();
                Assert.Single(ranges);
                Assert.Equal(10, ranges.First().Start);
                Assert.Equal(40, ranges.First().End);
                Assert.Equal(31, ranges.First().Count);
            }

            using (var set = new FileCache())
            {
                set.Add(25, new MemoryStream(new byte[16]));
                set.Add(15, new MemoryStream(new byte[16]));
                set.Add(10, new MemoryStream(new byte[11]));
                set.Add(0, new MemoryStream(new byte[101]));
                var ranges = set.GetRanges();
                Assert.Single(ranges);
                Assert.Equal(0, ranges.First().Start);
                Assert.Equal(100, ranges.First().End);
                Assert.Equal(101, ranges.First().Count);
            }

            using (var set = new FileCache())
            {
                set.Add(0, new MemoryStream(new byte[101]));
                set.Add(25, new MemoryStream(new byte[16]));
                set.Add(15, new MemoryStream(new byte[16]));
                set.Add(10, new MemoryStream(new byte[11]));
                var ranges = set.GetRanges();
                Assert.Single(ranges);
                Assert.Equal(0, ranges.First().Start);
                Assert.Equal(100, ranges.First().End);
                Assert.Equal(101, ranges.First().Count);
            }

            using (var set = new FileCache())
            {
                set.Add(10, new MemoryStream(new byte[11]));
                set.Add(30, new MemoryStream(new byte[11]));
                set.Add(50, new MemoryStream(new byte[11]));
                set.Add(0, new MemoryStream(new byte[101]));
                var ranges = set.GetRanges();
                Assert.Single(ranges);
                Assert.Equal(0, ranges.First().Start);
                Assert.Equal(100, ranges.First().End);
                Assert.Equal(101, ranges.First().Count);
            }

            using (var set = new FileCache())
            {
                set.Add(10, new MemoryStream(new byte[11]));
                set.Add(21, new MemoryStream(new byte[10]));
                set.Add(31, new MemoryStream(new byte[10]));
                var ranges = set.GetRanges();
                Assert.Single(ranges);
                Assert.Equal(10, ranges.First().Start);
                Assert.Equal(40, ranges.First().End);
                Assert.Equal(31, ranges.First().Count);
            }

            using (var set = new FileCache())
            {
                set.Add(31, new MemoryStream(new byte[10]));
                set.Add(21, new MemoryStream(new byte[10]));
                set.Add(10, new MemoryStream(new byte[11]));
                var ranges = set.GetRanges();
                Assert.Single(ranges);
                Assert.Equal(10, ranges.First().Start);
                Assert.Equal(40, ranges.First().End);
                Assert.Equal(31, ranges.First().Count);
            }

            using (var set = new FileCache())
            {
                set.Add(10, new MemoryStream(new byte[11]));
                set.Add(31, new MemoryStream(new byte[10]));
                set.Add(21, new MemoryStream(new byte[10]));
                var ranges = set.GetRanges();
                Assert.Single(ranges);
                Assert.Equal(10, ranges.First().Start);
                Assert.Equal(40, ranges.First().End);
                Assert.Equal(31, ranges.First().Count);
            }

            using (var set = new FileCache())
            {
                set.Add(21, new MemoryStream(new byte[10]));
                set.Add(10, new MemoryStream(new byte[11]));
                set.Add(31, new MemoryStream(new byte[10]));
                var ranges = set.GetRanges();
                Assert.Single(ranges);
                Assert.Equal(10, ranges.First().Start);
                Assert.Equal(40, ranges.First().End);
                Assert.Equal(31, ranges.First().Count);
            }
        }

        [Fact]
        public void MergeToMultiTest()
        {
            using (var set = new FileCache())
            {
                set.Add(10, new MemoryStream(new byte[11]));
                set.Add(21, new MemoryStream(new byte[10]));
                set.Add(31, new MemoryStream(new byte[10]));

                set.Add(50, new MemoryStream(new byte[11]));
                set.Add(61, new MemoryStream(new byte[10]));
                set.Add(71, new MemoryStream(new byte[10]));
                var ranges = set.GetRanges();
                Assert.Equal(2, ranges.Count);
                Assert.Equal(10, ranges.First().Start);
                Assert.Equal(40, ranges.First().End);
                Assert.Equal(31, ranges.First().Count);
                Assert.Equal(50, ranges.Last().Start);
                Assert.Equal(80, ranges.Last().End);
                Assert.Equal(31, ranges.Last().Count);
            }

            using (var set = new FileCache())
            {
                set.Add(21, new MemoryStream(new byte[10]));
                set.Add(10, new MemoryStream(new byte[11]));
                set.Add(31, new MemoryStream(new byte[10]));

                set.Add(61, new MemoryStream(new byte[10]));
                set.Add(50, new MemoryStream(new byte[11]));
                set.Add(71, new MemoryStream(new byte[10]));
                var ranges = set.GetRanges();
                Assert.Equal(2, ranges.Count);
                Assert.Equal(10, ranges.First().Start);
                Assert.Equal(40, ranges.First().End);
                Assert.Equal(31, ranges.First().Count);
                Assert.Equal(50, ranges.Last().Start);
                Assert.Equal(80, ranges.Last().End);
                Assert.Equal(31, ranges.Last().Count);
            }
        }

        [Fact]
        public void GetRamainingsTest()
        {
            using (var set = new FileCache())
            {
                set.Add(10, new MemoryStream(new byte[11]));
                set.Add(30, new MemoryStream(new byte[11]));
                set.Add(50, new MemoryStream(new byte[11]));
                set.Add(70, new MemoryStream(new byte[11]));

                var rems = set.GetRamainings(0, 101);
                Assert.Equal(5, rems.Length);
                Assert.Equal(0, rems[0].Start);
                Assert.Equal(9, rems[0].End);
                Assert.Equal(10, rems[0].Count);

                Assert.Equal(21, rems[1].Start);
                Assert.Equal(29, rems[1].End);
                Assert.Equal(9, rems[1].Count);

                Assert.Equal(41, rems[2].Start);
                Assert.Equal(49, rems[2].End);
                Assert.Equal(9, rems[2].Count);

                Assert.Equal(61, rems[3].Start);
                Assert.Equal(69, rems[3].End);
                Assert.Equal(9, rems[3].Count);

                Assert.Equal(81, rems[4].Start);
                Assert.Equal(100, rems[4].End);
                Assert.Equal(20, rems[4].Count);
            }

            using (var set = new FileCache())
            {
                set.Add(70, new MemoryStream(new byte[11]));
                set.Add(50, new MemoryStream(new byte[11]));
                set.Add(30, new MemoryStream(new byte[11]));
                set.Add(10, new MemoryStream(new byte[11]));

                var rems = set.GetRamainings(0, 101);
                Assert.Equal(5, rems.Length);
                Assert.Equal(0, rems[0].Start);
                Assert.Equal(9, rems[0].End);
                Assert.Equal(21, rems[1].Start);
                Assert.Equal(29, rems[1].End);
                Assert.Equal(41, rems[2].Start);
                Assert.Equal(49, rems[2].End);
                Assert.Equal(61, rems[3].Start);
                Assert.Equal(69, rems[3].End);
                Assert.Equal(81, rems[4].Start);
                Assert.Equal(100, rems[4].End);
            }

            using (var set = new FileCache())
            {
                set.Add(10, new MemoryStream(new byte[11]));
                set.Add(30, new MemoryStream(new byte[11]));
                set.Add(50, new MemoryStream(new byte[11]));
                set.Add(70, new MemoryStream(new byte[11]));
                Assert.Empty(set.GetRamainings(10, 11));
                Assert.Empty(set.GetRamainings(11, 10));
                Assert.Empty(set.GetRamainings(10, 6));
                Assert.Empty(set.GetRamainings(11, 9));

                var rems = set.GetRamainings(5, 11);
                Assert.Single(rems);
                Assert.Equal(5, rems[0].Start);
                Assert.Equal(9, rems[0].End);

                rems = set.GetRamainings(5, 16);
                Assert.Single(rems);
                Assert.Equal(5, rems[0].Start);
                Assert.Equal(9, rems[0].End);

                rems = set.GetRamainings(15, 11);
                Assert.Single(rems);
                Assert.Equal(21, rems[0].Start);
                Assert.Equal(25, rems[0].End);

                rems = set.GetRamainings(10, 21);
                Assert.Single(rems);
                Assert.Equal(21, rems[0].Start);
                Assert.Equal(29, rems[0].End);


                rems = set.GetRamainings(5, 21);
                Assert.Equal(2, rems.Length);
                Assert.Equal(5, rems[0].Start);
                Assert.Equal(9, rems[0].End);
                Assert.Equal(21, rems[1].Start);
                Assert.Equal(25, rems[1].End);
            }
        }
    }
}
