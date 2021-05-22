using System.IO;
using Xunit;

namespace EzSmbTest.Streams
{
    public class StreamTest
    {
        [Fact]
        public void MemoryStreamOperationTest()
        {
            var stream = new MemoryStream();
            stream.SetLength(10);
            Assert.Equal(10, stream.Length);

            stream.Position = 0;
            var readedByte = stream.ReadByte();
            Assert.Equal(0, readedByte);
            Assert.Equal(1, stream.Position);
        }
    }
}
