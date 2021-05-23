using EzSmb.Streams.Caches;
using Xunit;

namespace EzSmbTest.Streams.Caches
{
    /// <summary>
    /// Set Range class to public, and run.
    /// </summary>
    public class RangeTest
    {
        [Fact]
        public void CreateTest()
        {
            var range1 = new Range(1, 2);
            Assert.Equal(1, range1.Start);
            Assert.Equal(2, range1.End);

            var range2 = new Range(1, 1);
            Assert.Equal(1, range2.Start);
            Assert.Equal(1, range2.End);

            var range3 = new Range(3, 1);
            Assert.Equal(1, range3.Start);
            Assert.Equal(3, range3.End);
        }
    }
}
