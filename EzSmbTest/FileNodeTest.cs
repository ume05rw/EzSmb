using EzSmb;
using EzSmb.Params;
using EzSmb.Params.Enums;
using EzSmbTest.Bases;
using EzSmbTest.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EzSmbTest
{
    public class FileNodeTest : TestBase
    {
        public FileNodeTest() : base()
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

        [Fact]
        public async Task GetNodeTestRelatedServer()
        {
            foreach (var setting in this.Settings)
            {
                var relatedPath = setting.TestPath.File.RelatedServer;
                if (string.IsNullOrEmpty(relatedPath))
                    continue;

                var set = this.GetParamSet(setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    this.AssertNodeServer(await node.GetNode(relatedPath));
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    this.AssertNodeServer(await node.GetNode(relatedPath));
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    this.AssertNodeServer(await node.GetNode(relatedPath));
                }
            }
        }

        [Fact]
        public async Task GetNodeTestRelatedShare()
        {
            foreach (var setting in this.Settings)
            {
                var relatedPath = setting.TestPath.File.RelatedShare;
                if (string.IsNullOrEmpty(relatedPath))
                    continue;

                var set = this.GetParamSet(setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    this.AssertNodeShareFolder(await node.GetNode(relatedPath));
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    this.AssertNodeShareFolder(await node.GetNode(relatedPath));
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    this.AssertNodeShareFolder(await node.GetNode(relatedPath));
                }
            }
        }

        [Fact]
        public async Task GetNodeTestRelatedFolder()
        {
            foreach (var setting in this.Settings)
            {
                var relatedPath = setting.TestPath.File.RelatedFolder;
                if (string.IsNullOrEmpty(relatedPath))
                    continue;

                var set = this.GetParamSet(setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    this.AssertNodeSubFolder(await node.GetNode(relatedPath));
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    this.AssertNodeSubFolder(await node.GetNode(relatedPath));
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    this.AssertNodeSubFolder(await node.GetNode(relatedPath));
                }
            }
        }

        [Fact]
        public async Task GetNodeTestRelatedFile()
        {
            foreach (var setting in this.Settings)
            {
                var relatedPath = setting.TestPath.File.RelatedFile;
                if (string.IsNullOrEmpty(relatedPath))
                    continue;

                var set = this.GetParamSet(setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    this.AssertNodeFile(await node.GetNode(relatedPath));
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    this.AssertNodeFile(await node.GetNode(relatedPath));
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    this.AssertNodeFile(await node.GetNode(relatedPath));
                }
            }
        }


        [Fact]
        public async Task GetListFailTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    this.AssertErrorResult(await node.GetList(), node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    this.AssertErrorResult(await node.GetList(), node);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    this.AssertErrorResult(await node.GetList(), node);
                }
            }
        }

        [Fact]
        public async Task ReadTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                set.SmbType = null;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    var result = await node.Read();
                    Assert.False(node.HasError, string.Join(", ", node.Errors));
                    Assert.NotNull(result);
                    Assert.Equal(0, result.Position);
                }

                set.SmbType = SmbType.Smb1;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb1)
                {
                    var result = await node.Read();
                    Assert.False(node.HasError, string.Join(", ", node.Errors));
                    Assert.NotNull(result);
                    Assert.Equal(0, result.Position);
                }

                set.SmbType = SmbType.Smb2;
                node = await this.GetNode(setting, set);
                if (setting.SupportedSmb2)
                {
                    var result = await node.Read();
                    Assert.False(node.HasError, string.Join(", ", node.Errors));
                    Assert.NotNull(result);
                    Assert.Equal(0, result.Position);
                }
            }
        }

        [Fact]
        public async Task CreateReadOverwriteDeleteTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = (await this.GetNode(setting, set)).GetParent().GetParent();
                    await this.InnerCreateReadOverwriteDeleteTest(node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = (await this.GetNode(setting, set)).GetParent();
                    await this.InnerCreateReadOverwriteDeleteTest(node);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = (await this.GetNode(setting, set)).GetParent();
                    await this.InnerCreateReadOverwriteDeleteTest(node);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "<保留中>")]
        private async Task InnerCreateReadOverwriteDeleteTest(Node node)
        {
            var newFileName = "てすとだよ！.txt";
            var createBytes = Encoding.UTF8.GetBytes("日本語だYO!\r\n改行だよ\nLFで改行したよ。");
            var createStream = new MemoryStream(createBytes);
            var updateBytes = Encoding.UTF8.GetBytes("update all text.\nok?");
            var updateStream = new MemoryStream(updateBytes);

            var created = await node.Write(createStream, newFileName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            Assert.True(created);

            var createdStream = await node.Read(newFileName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            Assert.True(createBytes.SequenceEqual(createdStream.ToArray()));

            var updated = await node.Write(updateStream, newFileName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            Assert.True(updated);

            var updatedStream = await node.Read(newFileName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            Assert.True(updateBytes.SequenceEqual(updatedStream.ToArray()));

            var deleted = await node.Delete(newFileName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            Assert.True(deleted);
        }

        [Fact]
        public async Task CreateReadOverwriteMoveDeleteTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                // SMB1 Not Supported.
                //if (setting.SupportedSmb1 || setting.SupportedSmb2)
                //{
                //    set.SmbType = null;
                //    node = (await this.GetNode(setting, set)).GetParent().GetParent();
                //    await this.InnerCreateReadOverwriteMoveDeleteTest(node);
                //}

                //if (setting.SupportedSmb1)
                //{
                //    set.SmbType = SmbType.Smb1;
                //    node = (await this.GetNode(setting, set)).GetParent();
                //    await this.InnerCreateReadOverwriteMoveDeleteTest(node);
                //}

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = (await this.GetNode(setting, set)).GetParent();
                    await this.InnerCreateReadOverwriteMoveDeleteTest(node);
                }
            }
        }

        private async Task InnerCreateReadOverwriteMoveDeleteTest(Node node)
        {
            var newFileName = "do_renaming!.txt";
            var createBytes = Encoding.UTF8.GetBytes("日本語だYO!\r\n改行だよ\nLFで改行したよ。");
            var createStream = new MemoryStream(createBytes);
            var updateBytes = Encoding.UTF8.GetBytes("update all text.\nok?");
            var updateStream = new MemoryStream(updateBytes);
            var renameName = "renamed.log";

            var created = await node.Write(createStream, newFileName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            Assert.True(created);

            var createdStream = await node.Read(newFileName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            Assert.True(createBytes.SequenceEqual(createdStream.ToArray()));

            var updated = await node.Write(updateStream, newFileName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            Assert.True(updated);

            var updatedStream = await node.Read(newFileName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            Assert.True(updateBytes.SequenceEqual(updatedStream.ToArray()));

            var renamed = await node.Move(renameName, newFileName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            this.AssertNodeFile(renamed);

            var renamedStream = await node.Read(renameName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            Assert.True(updateBytes.SequenceEqual(renamedStream.ToArray()));

            var deleted = await node.Delete(renameName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            Assert.True(deleted);
        }

        [Fact]
        public async Task MoveTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                // SMB1 Not Supported.
                //if (setting.SupportedSmb1 || setting.SupportedSmb2)
                //{
                //    set.SmbType = null;
                //    node = (await this.GetNode(setting, set)).GetParent().GetParent();
                //    await this.InnerMoveTest(node);
                //}

                //if (setting.SupportedSmb1)
                //{
                //    set.SmbType = SmbType.Smb1;
                //    node = (await this.GetNode(setting, set)).GetParent();
                //    await this.InnerMoveTest(node);
                //}

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = (await this.GetNode(setting, set)).GetParent();
                    await this.InnerMoveTest(node);
                }
            }
        }

        private async Task InnerMoveTest(Node node)
        {
            var newFileName = "てすとだよ！2.txt";
            var createBytes = Encoding.UTF8.GetBytes("日本語だYO!\r\n改行だよ\nLFで改行したよ。");
            var createStream = new MemoryStream(createBytes);
            var movePath = "../moved!.log";

            var created = await node.Write(createStream, newFileName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            Assert.True(created);

            var newNode = await node.GetNode(newFileName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            this.AssertNodeFile(newNode);

            var createdStream = await newNode.Read();
            Assert.False(newNode.HasError, string.Join(", ", newNode.Errors));
            Assert.True(createBytes.SequenceEqual(createdStream.ToArray()));

            var movedNode = await newNode.Move(movePath);
            Assert.False(movedNode.HasError, string.Join(", ", movedNode.Errors));

            var deleted = await movedNode.Delete();
            Assert.False(movedNode.HasError, string.Join(", ", movedNode.Errors));
            Assert.True(deleted);
        }
    }
}
