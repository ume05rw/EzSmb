using EzSmb;
using EzSmb.Params;
using EzSmb.Params.Enums;
using EzSmb.Streams;
using EzSmbTest.Bases;
using EzSmbTest.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EzSmbTest.Streams
{
    public class ReaderStreamTest : TestBase
    {
        public ReaderStreamTest() : base()
        {
        }

        /// <summary>
        /// このファイルで取得するNodeは全て、FileNode
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        private async Task<Node> GetNode(Setting setting, ParamSet set)
        {
            var path = $@"{setting.Address}\{setting.TestPath.File.Path}";
            var node = await Node.GetNode(path, set);

            if (set.SmbType == null)
            {
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    this.AssertNodeFile(node);
                else
                    Assert.Null(node);
            }
            else if (set.SmbType == SmbType.Smb1)
            {
                if (setting.SupportedSmb1)
                    this.AssertNodeFile(node);
                else
                    Assert.Null(node);
            }
            else if (set.SmbType == SmbType.Smb2)
            {
                if (setting.SupportedSmb2)
                    this.AssertNodeFile(node);
                else
                    Assert.Null(node);
            }

            return node;
        }

        private async Task<Node> GetServerNode(Setting setting, ParamSet set)
        {
            var node = await Node.GetNode(setting.Address, set);

            if (set.SmbType == null)
            {
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    this.AssertNodeServer(node);
                else
                    Assert.Null(node);
            }
            else if (set.SmbType == SmbType.Smb1)
            {
                if (setting.SupportedSmb1)
                    this.AssertNodeServer(node);
                else
                    Assert.Null(node);
            }
            else if (set.SmbType == SmbType.Smb2)
            {
                if (setting.SupportedSmb2)
                    this.AssertNodeServer(node);
                else
                    Assert.Null(node);
            }

            return node;
        }

        [Fact]
        public async Task CreateTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                set.SmbType = null;
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    try
                    {
                        node = await this.GetNode(setting, set);
                        using var readed = await node.Read();
                        using var stream = node.GetReaderStream();
                        this.InnerCreateTest(stream, readed);
                    }
                    catch (Exception ex)
                    {
                        Assert.True(false, $"Exception on Create.: {ex.Message}, {ex.StackTrace}");
                    }
                }

                set.SmbType = SmbType.Smb1;
                if (setting.SupportedSmb1)
                {
                    try
                    {
                        node = await this.GetNode(setting, set);
                        using var readed = await node.Read();
                        using var stream = node.GetReaderStream();
                        this.InnerCreateTest(stream, readed);
                    }
                    catch (Exception ex)
                    {
                        Assert.True(false, $"Exception on Create.: {ex.Message}, {ex.StackTrace}");
                    }
                }

                set.SmbType = SmbType.Smb2;
                if (setting.SupportedSmb2)
                {
                    try
                    {
                        node = await this.GetNode(setting, set);
                        using var readed = await node.Read();
                        using var stream = node.GetReaderStream();
                        this.InnerCreateTest(stream, readed);
                    }
                    catch (Exception ex)
                    {
                        Assert.True(false, $"Exception on Create.: {ex.Message}, {ex.StackTrace}");
                    }
                }
            }
        }

        [Fact]
        public async Task CreateRelativeTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                set.SmbType = null;
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    try
                    {
                        node = await this.GetServerNode(setting, set);
                        using var readed = await node.Read(setting.TestPath.RelatedFile.Path);
                        using var stream = await node.GetReaderStream(setting.TestPath.RelatedFile.Path);
                        this.InnerCreateTest(stream, readed);
                    }
                    catch (Exception ex)
                    {
                        Assert.True(false, $"Exception on Create.: {ex.Message}, {ex.StackTrace}");
                    }
                }

                set.SmbType = SmbType.Smb1;

                if (setting.SupportedSmb1)
                {
                    try
                    {
                        node = await this.GetServerNode(setting, set);
                        using var readed = await node.Read(setting.TestPath.RelatedFile.Path);
                        using var stream = await node.GetReaderStream(setting.TestPath.RelatedFile.Path);
                        this.InnerCreateTest(stream, readed);
                    }
                    catch (Exception ex)
                    {
                        Assert.True(false, $"Exception on Create.: {ex.Message}, {ex.StackTrace}");
                    }
                }

                set.SmbType = SmbType.Smb2;
                if (setting.SupportedSmb2)
                {
                    try
                    {
                        node = await this.GetServerNode(setting, set);
                        using var readed = await node.Read(setting.TestPath.RelatedFile.Path);
                        using var stream = await node.GetReaderStream(setting.TestPath.RelatedFile.Path);
                        this.InnerCreateTest(stream, readed);
                    }
                    catch (Exception ex)
                    {
                        Assert.True(false, $"Exception on Create.: {ex.Message}, {ex.StackTrace}");
                    }
                }
            }
        }

        [Fact]
        public async Task CreateStaticTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.Address}/{setting.TestPath.File.Path}";
                Node node;

                set.SmbType = null;
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    try
                    {
                        node = await Node.GetNode(path, set);
                        using var readed = await node.Read();
                        using var stream = node.GetReaderStream();
                        this.InnerCreateTest(stream, readed);
                    }
                    catch (Exception ex)
                    {
                        Assert.True(false, $"Exception on Create.: {ex.Message}, {ex.StackTrace}");
                    }
                }

                set.SmbType = SmbType.Smb1;
                if (setting.SupportedSmb1)
                {
                    try
                    {
                        node = await Node.GetNode(path, set);
                        using var readed = await node.Read();
                        using var stream = node.GetReaderStream();
                        this.InnerCreateTest(stream, readed);
                    }
                    catch (Exception ex)
                    {
                        Assert.True(false, $"Exception on Create.: {ex.Message}, {ex.StackTrace}");
                    }
                }

                set.SmbType = SmbType.Smb2;
                if (setting.SupportedSmb2)
                {
                    try
                    {
                        node = await Node.GetNode(path, set);
                        using var readed = await node.Read();
                        using var stream = node.GetReaderStream();
                        this.InnerCreateTest(stream, readed);
                    }
                    catch (Exception ex)
                    {
                        Assert.True(false, $"Exception on Create.: {ex.Message}, {ex.StackTrace}");
                    }
                }
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "<保留中>")]
        private void InnerCreateTest(ReaderStream stream, MemoryStream readedStream)
        {
            Assert.NotNull(stream);
            Assert.True(stream.CanRead);
            Assert.True(stream.CanSeek);
            Assert.False(stream.CanWrite);
            Assert.True(stream.CanTimeout);
            Assert.Equal(0, stream.Position);
            Assert.False(stream.IsUseCache);
            Assert.Equal(readedStream.Length, stream.Length);
        }

        [Fact]
        public async Task FlushTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                set.SmbType = null;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    await this.InnerFlushTest(node);

                set.SmbType = SmbType.Smb1;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb1)
                    await this.InnerFlushTest(node);

                set.SmbType = SmbType.Smb2;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb2)
                    await this.InnerFlushTest(node);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "<保留中>")]
        private async Task InnerFlushTest(Node node)
        {
            using var stream = node.GetReaderStream();
            try
            {
                stream.IsUseCache = false;
                stream.Flush();

                stream.IsUseCache = true;
                stream.Flush();

                stream.IsUseCache = false;
                await stream.FlushAsync();

                stream.IsUseCache = true;
                await stream.FlushAsync();
            }
            catch (Exception ex)
            {
                Assert.True(false, $"Exception on Flush.: {ex.Message}, {ex.StackTrace}");
            }
        }

        [Fact]
        public async Task SeekAndSetPositionTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                set.SmbType = null;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    await this.InnerSeekAndSetPositionTest(node);

                set.SmbType = SmbType.Smb1;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb1)
                    await this.InnerSeekAndSetPositionTest(node);

                set.SmbType = SmbType.Smb2;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb2)
                    await this.InnerSeekAndSetPositionTest(node);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "<保留中>")]
        private async Task InnerSeekAndSetPositionTest(Node node)
        {
            using var readed = await node.Read();
            using var stream = node.GetReaderStream();
            Assert.Equal(0, stream.Position);

            Assert.Equal(10, stream.Seek(10, SeekOrigin.Begin));
            Assert.Equal(10, stream.Position);

            Assert.Equal(readed.Length - 10, stream.Seek(-10, SeekOrigin.End));
            Assert.Equal(readed.Length - 10, stream.Position);

            stream.Position = 0;
            Assert.Equal(0, stream.Position);

            Assert.Equal(20, stream.Seek(20, SeekOrigin.Current));
            Assert.Equal(20, stream.Position);

            Assert.Equal(50, stream.Seek(30, SeekOrigin.Current));
            Assert.Equal(50, stream.Position);

            Assert.Equal(45, stream.Seek(-5, SeekOrigin.Current));
            Assert.Equal(45, stream.Position);


            stream.Position = 10;
            Assert.Equal(10, stream.Position);

            stream.Position = 20;
            Assert.Equal(20, stream.Position);

            stream.Position = readed.Length + 10;
            Assert.Equal(readed.Length, stream.Position);

            stream.Position = -10;
            Assert.Equal(0, stream.Position);
        }

        [Fact]
        public async Task NotSupportedMethodsTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                set.SmbType = null;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    await this.InnerNotSupportedMethodsTest(node);

                set.SmbType = SmbType.Smb1;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb1)
                    await this.InnerNotSupportedMethodsTest(node);

                set.SmbType = SmbType.Smb2;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb2)
                    await this.InnerNotSupportedMethodsTest(node);
            }
        }

        private async Task InnerNotSupportedMethodsTest(Node node)
        {
            using var stream = node.GetReaderStream();
            try
            {
                stream.SetLength(10);
                Assert.True(false, "ReaderStream.SetLength Not Throws.");
            }
            catch (Exception ex)
            {
                this.Dump(ex);
            }

            try
            {
                stream.Write(new byte[10], 0, 10);
                Assert.True(false, "ReaderStream.Write Not Throws.");
            }
            catch (Exception ex)
            {
                this.Dump(ex);
            }

            try
            {
                await stream.WriteAsync((new byte[10]).AsMemory(0, 10));
                Assert.True(false, "ReaderStream.WriteAsync Not Throws.");
            }
            catch (Exception ex)
            {
                this.Dump(ex);
            }

            try
            {
                stream.WriteByte(2);
                Assert.True(false, "ReaderStream.WriteByte Not Throws.");
            }
            catch (Exception ex)
            {
                this.Dump(ex);
            }

            try
            {
                stream.BeginWrite(new byte[10], 0, 10, null, null);
                Assert.True(false, "ReaderStream.BeginWrite Not Throws.");
            }
            catch (Exception ex)
            {
                this.Dump(ex);
            }

            try
            {
                stream.WriteTimeout = 1000;
                Assert.True(false, "ReaderStream.WriteTimeout Property Not Throws.");
            }
            catch (Exception ex)
            {
                this.Dump(ex);
            }
        }

        [Fact]
        public async Task ReadWithoutCacheTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node serverNode;
                Node fileNode;

                set.SmbType = null;
                serverNode = await this.GetServerNode(setting, set);
                fileNode = await serverNode.GetNode(setting.TestPath.RelatedFile.Path);
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    await this.InnerReadWithoutCacheTest(fileNode);

                set.SmbType = SmbType.Smb1;
                serverNode = await this.GetServerNode(setting, set);
                fileNode = await serverNode.GetNode(setting.TestPath.RelatedFile.Path);
                if (setting.SupportedSmb1)
                    await this.InnerReadWithoutCacheTest(fileNode);

                set.SmbType = SmbType.Smb2;
                serverNode = await this.GetServerNode(setting, set);
                fileNode = await serverNode.GetNode(setting.TestPath.RelatedFile.Path);
                if (setting.SupportedSmb2)
                    await this.InnerReadWithoutCacheTest(fileNode);
            }
        }

        private async Task InnerReadWithoutCacheTest(Node node)
        {
            using var stream = node.GetReaderStream();
            using var readed = await node.Read();
            stream.IsUseCache = false;
            var fullBytes = readed.ToArray();
            var readedBytes1 = fullBytes.Take(5243080).ToArray();
            var readedBytes2 = fullBytes.Skip(3650).Take(1048576).ToArray();

            var buffer1 = new byte[5243080];
            var buffer2 = new byte[1048576];
            var startTime = default(DateTime);

            startTime = DateTime.Now;
            stream.Read(buffer1, 0, 5243080);
            this.Dump($"{node.ParamSet.SmbType} Read 5243080 bytes: {(DateTime.Now - startTime).TotalMilliseconds} msec.");
            Assert.True(readedBytes1.SequenceEqual(buffer1));
            Assert.Equal(5243080, stream.Position);

            stream.Seek(3650, SeekOrigin.Begin);
            Assert.Equal(3650, stream.Position);
            startTime = DateTime.Now;
            stream.Read(buffer2, 0, 1048576);
            this.Dump($"{node.ParamSet.SmbType} Read 1048576 bytes: {(DateTime.Now - startTime).TotalMilliseconds} msec.");
            Assert.True(readedBytes2.SequenceEqual(buffer2));
            Assert.Equal((3650 + 1048576), stream.Position);
        }

        [Fact]
        public async Task ReadWithCacheTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node serverNode;
                Node fileNode;

                set.SmbType = null;
                serverNode = await this.GetServerNode(setting, set);
                fileNode = await serverNode.GetNode(setting.TestPath.RelatedFile.Path);
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    await this.InnerReadWithCacheTest(fileNode);

                set.SmbType = SmbType.Smb1;
                serverNode = await this.GetServerNode(setting, set);
                fileNode = await serverNode.GetNode(setting.TestPath.RelatedFile.Path);
                if (setting.SupportedSmb1)
                    await this.InnerReadWithCacheTest(fileNode);

                set.SmbType = SmbType.Smb2;
                serverNode = await this.GetServerNode(setting, set);
                fileNode = await serverNode.GetNode(setting.TestPath.RelatedFile.Path);
                if (setting.SupportedSmb2)
                    await this.InnerReadWithCacheTest(fileNode);
            }
        }

        private async Task InnerReadWithCacheTest(Node node)
        {
            using var stream = node.GetReaderStream();
            using var readed = await node.Read();
            stream.IsUseCache = true;
            var fullBytes = readed.ToArray();
            var readedBytes1 = fullBytes.Take(5243080).ToArray();
            var readedBytes2 = fullBytes.Skip(3650).Take(1048576).ToArray();

            var buffer1 = new byte[5243080];
            var buffer2 = new byte[1048576];
            var startTime = default(DateTime);

            startTime = DateTime.Now;
            stream.Read(buffer1, 0, 5243080);
            this.Dump($"{node.ParamSet.SmbType} Read 5243080 bytes: {(DateTime.Now - startTime).TotalMilliseconds} msec.");
            Assert.True(readedBytes1.SequenceEqual(buffer1));
            Assert.Equal(5243080, stream.Position);

            // All the contents of the second Read are cached.
            stream.Seek(3650, SeekOrigin.Begin);
            Assert.Equal(3650, stream.Position);
            startTime = DateTime.Now;
            stream.Read(buffer2, 0, 1048576);
            this.Dump($"{node.ParamSet.SmbType} Read 1048576 bytes: {(DateTime.Now - startTime).TotalMilliseconds} msec.");
            Assert.True(readedBytes2.SequenceEqual(buffer2));
            Assert.Equal((3650 + 1048576), stream.Position);
        }

        [Fact]
        public async Task ReadWithShreddedCacheTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node serverNode;
                Node fileNode;

                set.SmbType = null;
                serverNode = await this.GetServerNode(setting, set);
                fileNode = await serverNode.GetNode(setting.TestPath.RelatedFile.Path);
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    await this.InnerReadWithShreddedCacheTest(fileNode);

                set.SmbType = SmbType.Smb1;
                serverNode = await this.GetServerNode(setting, set);
                fileNode = await serverNode.GetNode(setting.TestPath.RelatedFile.Path);
                if (setting.SupportedSmb1)
                    await this.InnerReadWithShreddedCacheTest(fileNode);

                set.SmbType = SmbType.Smb2;
                serverNode = await this.GetServerNode(setting, set);
                fileNode = await serverNode.GetNode(setting.TestPath.RelatedFile.Path);
                if (setting.SupportedSmb2)
                    await this.InnerReadWithShreddedCacheTest(fileNode);
            }
        }

        private async Task InnerReadWithShreddedCacheTest(Node node)
        {
            using var stream = node.GetReaderStream();
            using var readed = await node.Read();
            stream.IsUseCache = true;
            var fullBytes = readed.ToArray();


            var shreddedSize = 10000;
            var totalSize = 5243080;
            var gap = 1000;
            var readedBytes = fullBytes.Take(totalSize).ToArray();
            var shreddedCount = (int)((decimal)totalSize / (decimal)(shreddedSize + gap));
            var shreddedBuffer = new byte[shreddedSize];
            var totalBuffer = new byte[totalSize];
            var startTime = default(DateTime);

            startTime = DateTime.Now;
            for (var i = 0; i < shreddedCount; i++)
            {
                stream.Position = (shreddedSize + gap) * i;
                stream.Read(shreddedBuffer, 0, shreddedSize);
            }
            this.Dump($"{node.ParamSet.SmbType} Reads {shreddedCount} times, total {shreddedSize * shreddedCount} bytes: {(DateTime.Now - startTime).TotalMilliseconds} msec.");

            // Create lots of caches with small gaps.
            startTime = DateTime.Now;
            stream.Position = 0;
            stream.Read(totalBuffer, 0, totalSize);
            this.Dump($"{node.ParamSet.SmbType} Read {totalSize} bytes: {(DateTime.Now - startTime).TotalMilliseconds} msec.");
            Assert.True(readedBytes.SequenceEqual(totalBuffer));
            Assert.Equal(totalSize, stream.Position);
        }


        [Fact]
        public async Task ReadByteTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                set.SmbType = null;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    await this.InnerReadByteTest(node);

                set.SmbType = SmbType.Smb1;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb1)
                    await this.InnerReadByteTest(node);

                set.SmbType = SmbType.Smb2;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb2)
                    await this.InnerReadByteTest(node);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "<保留中>")]
        private async Task InnerReadByteTest(Node node)
        {
            using var stream = node.GetReaderStream();
            using var readed = await node.Read();
            stream.IsUseCache = false;
            var fullBytes = readed.ToArray();
            var readedByte = fullBytes.Skip(1200).Take(1).First();

            stream.Position = 1200;
            var readByte = stream.ReadByte();
            Assert.Equal(readedByte, readByte);
            Assert.Equal(1201, stream.Position);
        }

        [Fact]
        public async Task ReadAsyncTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                set.SmbType = null;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    await this.InnerReadAsyncTest(node);

                set.SmbType = SmbType.Smb1;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb1)
                    await this.InnerReadAsyncTest(node);

                set.SmbType = SmbType.Smb2;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb2)
                    await this.InnerReadAsyncTest(node);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "<保留中>")]
        private async Task InnerReadAsyncTest(Node node)
        {
            using var stream = node.GetReaderStream();
            using var readed = await node.Read();
            stream.IsUseCache = false;
            var fullBytes = readed.ToArray();
            var readedBytes = fullBytes.Skip(1200).Take(30).ToArray();
            var buffer = new byte[30];

            stream.Position = 1200;
            var readByte = await stream.ReadAsync(buffer.AsMemory(0, 30));
            Assert.True(readedBytes.SequenceEqual(buffer));
            Assert.Equal(1230, stream.Position);
        }

        [Fact]
        public async Task BeginReadTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                set.SmbType = null;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    await this.InnerBeginReadTest(node);

                set.SmbType = SmbType.Smb1;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb1)
                    await this.InnerBeginReadTest(node);

                set.SmbType = SmbType.Smb2;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb2)
                    await this.InnerBeginReadTest(node);
            }
        }

        private async Task InnerBeginReadTest(Node node)
        {
            using var readed = await node.Read();
            var fullBytes = readed.ToArray();

            using var stream = node.GetReaderStream();
            var readedBytes = fullBytes.Skip(1200).Take(30).ToArray();
            var buffer = new byte[30];

            stream.IsUseCache = false;
            stream.Position = 1200;
            var completion = new TaskCompletionSource();
            void callback(IAsyncResult ar)
            {
                var bytesRead = stream.EndRead(ar);
                if (30 <= bytesRead)
                {
                    completion.TrySetResult();
                }
                else
                {
                    stream.BeginRead(buffer, 0, 30, callback, null);
                }
            }

            stream.BeginRead(buffer, 0, 30, callback, null);

            await completion.Task;

            Assert.True(readedBytes.SequenceEqual(buffer));
            Assert.Equal(1230, stream.Position);
        }

        [Fact]
        public async Task ReadTimeoutTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node serverNode;
                Node fileNode;

                set.SmbType = null;
                serverNode = await this.GetServerNode(setting, set);
                fileNode = await serverNode.GetNode(setting.TestPath.RelatedFile.Path);
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    await this.InnerReadTimeoutTest(fileNode);

                set.SmbType = SmbType.Smb1;
                serverNode = await this.GetServerNode(setting, set);
                fileNode = await serverNode.GetNode(setting.TestPath.RelatedFile.Path);
                if (setting.SupportedSmb1)
                    await this.InnerReadTimeoutTest(fileNode);

                set.SmbType = SmbType.Smb2;
                serverNode = await this.GetServerNode(setting, set);
                fileNode = await serverNode.GetNode(setting.TestPath.RelatedFile.Path);
                if (setting.SupportedSmb2)
                    await this.InnerReadTimeoutTest(fileNode);
            }
        }

        private async Task InnerReadTimeoutTest(Node node)
        {
            using var stream = node.GetReaderStream();
            using var readed = await node.Read();
            stream.IsUseCache = true;
            var fullBytes = readed.ToArray();

            stream.ReadTimeout = 0;

            var shreddedSize = 10000;
            var totalSize = 5243080;
            var gap = 1000;
            var readedBytes = fullBytes.Take(totalSize).ToArray();
            var shreddedCount = (int)((decimal)totalSize / (decimal)(shreddedSize + gap));
            var shreddedBuffer = new byte[shreddedSize];
            var totalBuffer = new byte[totalSize];
            var startTime = default(DateTime);

            // Create lots of caches with small gaps.
            for (var i = 0; i < shreddedCount; i++)
            {
                stream.Position = (shreddedSize + gap) * i;
                stream.Read(shreddedBuffer, 0, shreddedSize);
            }

            this.Dump($"Timeout Test Start.");
            startTime = DateTime.Now;
            stream.ReadTimeout = 200;
            try
            {
                stream.Position = 0;
                stream.Read(totalBuffer, 0, totalSize);
                Assert.True(false, "Timeout Exception Not throwed.");
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Assert.True(false, $"Unexpected Exception: {ex}, {ex.Message}, {ex.StackTrace}");
            }

            this.Dump($"Timeout Test Succeeded.: {(DateTime.Now - startTime).TotalMilliseconds} msec.");
        }
    }
}
