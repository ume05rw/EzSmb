using EzSmb;
using EzSmb.Params;
using EzSmb.Params.Enums;
using EzSmbTest.Bases;
using EzSmbTest.Models;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EzSmbTest
{
    public class SubFolderNodeTest : TestBase
    {
        public SubFolderNodeTest() : base()
        {
        }

        /// <summary>
        /// このファイルで取得するNodeは全て、共有フォルダでない FolderNode.
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        private async Task<Node> GetNode(Setting setting, ParamSet set)
        {
            var path = $@"{setting.Address}\{setting.TestPath.Folder}";
            var node = await Node.GetNode(path, set);

            if (set.SmbType == null)
            {
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    this.AssertNodeSubFolder(node);
                else
                    Assert.Null(node);
            }
            else if (set.SmbType == SmbType.Smb1)
            {
                if (setting.SupportedSmb1)
                    this.AssertNodeSubFolder(node);
                else
                    Assert.Null(node);
            }
            else if (set.SmbType == SmbType.Smb2)
            {
                if (setting.SupportedSmb2)
                    this.AssertNodeSubFolder(node);
                else
                    Assert.Null(node);
            }

            return node;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "<保留中>")]
        private Related GetRelated(Related from, Setting setting)
        {
            return new Related()
            {
                Path = from.Path[(setting.TestPath.Folder.Length + 1)..],
                Name = from.Name
            };
        }

        [Fact]
        public async Task GetListTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    await this.AssertGetList(node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    await this.AssertGetList(node);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    await this.AssertGetList(node);
                }
            }
        }

        [Fact]
        public async Task GetNodeTestFile()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = setting.TestPath.File[(setting.TestPath.Folder.Length + 1)..];
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    this.AssertNodeFile(await node.GetNode(path));
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    this.AssertNodeFile(await node.GetNode(path));
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    this.AssertNodeFile(await node.GetNode(path));
                }
            }
        }

        [Fact]
        public async Task GetNodeTestRelatedServer()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var related = this.GetRelated(setting.TestPath.RelatedServer, setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    await this.AssertRelatedServer(node, related);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    await this.AssertRelatedServer(node, related);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    await this.AssertRelatedServer(node, related);
                }
            }
        }

        [Fact]
        public async Task GetNodeTestRelatedShareFolder()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var related = this.GetRelated(setting.TestPath.RelatedShare, setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    await this.AssertRelatedShareFolder(node, related);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    await this.AssertRelatedShareFolder(node, related);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    await this.AssertRelatedShareFolder(node, related);
                }
            }
        }

        [Fact]
        public async Task GetNodeTestRelatedSubFolder()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var related = this.GetRelated(setting.TestPath.RelatedFolder, setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    await this.AssertRelatedSubFolder(node, related);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    await this.AssertRelatedSubFolder(node, related);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    await this.AssertRelatedSubFolder(node, related);
                }
            }
        }

        [Fact]
        public async Task GetNodeTestRelatedFile()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var related = this.GetRelated(setting.TestPath.RelatedFile, setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    await this.AssertRelatedFile(node, related);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    await this.AssertRelatedFile(node, related);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    await this.AssertRelatedFile(node, related);
                }
            }
        }

        [Fact]
        public async Task CreateDeleteTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    await this.InnerCreateDeleteTest(node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    await this.InnerCreateDeleteTest(node);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    await this.InnerCreateDeleteTest(node);
                }
            }
        }

        private async Task InnerCreateDeleteTest(Node node)
        {
            var createName = "created1";

            var created = await node.CreateFolder(createName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            this.AssertNodeSubFolder(created);

            var deleted = await created.Delete();
            Assert.False(created.HasError, string.Join(", ", created.Errors));
            Assert.True(deleted);
        }

        [Fact]
        public async Task CreateMoveDeleteTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                // SMB1 Not Supported.
                //if (setting.SupportedSmb1 || setting.SupportedSmb2)
                //{
                //    set.SmbType = null;
                //    node = await this.GetNode(setting, set);
                //    await this.InnerCreateMoveDeleteTest(node);
                //}

                //if (setting.SupportedSmb1)
                //{
                //    set.SmbType = SmbType.Smb1;
                //    node = await this.GetNode(setting, set);
                //    await this.InnerCreateMoveDeleteTest(node);
                //}

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    await this.InnerCreateMoveDeleteTest(node);
                }
            }
        }

        private async Task InnerCreateMoveDeleteTest(Node node)
        {
            var createName = "created2";
            var updateName = "updated2";

            var created = await node.CreateFolder(createName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            this.AssertNodeSubFolder(created);

            var updated = await created.Move(updateName);
            Assert.False(created.HasError, string.Join(", ", created.Errors));
            this.AssertNodeSubFolder(updated);

            var deleted = await updated.Delete();
            Assert.False(updated.HasError, string.Join(", ", updated.Errors));
            Assert.True(deleted);
        }

        [Fact]
        public async Task CreateMoveDeleteTestOnFolderHasFile()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                // SMB1 Not Supported.
                //if (setting.SupportedSmb1 || setting.SupportedSmb2)
                //{
                //    set.SmbType = null;
                //    node = await this.GetNode(setting, set);
                //    await this.InnerCreateMoveDeleteTestOnFolderHasFile(node);
                //}

                //if (setting.SupportedSmb1)
                //{
                //    set.SmbType = SmbType.Smb1;
                //    node = await this.GetNode(setting, set);
                //    await this.InnerCreateMoveDeleteTestOnFolderHasFile(node);
                //}

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    await this.InnerCreateMoveDeleteTestOnFolderHasFile(node);
                }
            }
        }

        private async Task InnerCreateMoveDeleteTestOnFolderHasFile(Node node)
        {
            var createName = "created3";
            var updateName = "updated3";
            var fileName = "fileOnFolder.txt";
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("file!"));

            var created = await node.CreateFolder(createName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            this.AssertNodeSubFolder(created);

            var writedFile = await created.Write(fileStream, fileName);
            Assert.False(created.HasError, string.Join(", ", created.Errors));
            Assert.True(writedFile);

            var updated = await created.Move(updateName);
            Assert.False(created.HasError, string.Join(", ", created.Errors));
            this.AssertNodeSubFolder(updated);

            // Always fail if exists file.
            Assert.False(await updated.Delete());
            Assert.True(updated.HasError);

            updated.ClearErrors();
            var fileDeleted = await updated.Delete(fileName);
            Assert.False(created.HasError, string.Join(", ", created.Errors));
            Assert.True(fileDeleted);

            var folderDeleted = await updated.Delete();
            Assert.False(updated.HasError, string.Join(", ", updated.Errors));
            Assert.True(folderDeleted);
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
                //    node = await this.GetNode(setting, set);
                //    await this.InnerMoveTest(node);
                //}

                //if (setting.SupportedSmb1)
                //{
                //    set.SmbType = SmbType.Smb1;
                //    node = await this.GetNode(setting, set);
                //    await this.InnerMoveTest(node);
                //}

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    await this.InnerMoveTest(node);
                }
            }
        }

        private async Task InnerMoveTest(Node node)
        {
            var createName = "created4";
            var updateName = "../updated4";
            var fileName = "fileOnFolder.txt";
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("file!"));

            var created = await node.CreateFolder(createName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            this.AssertNodeSubFolder(created);

            var writedFile = await created.Write(fileStream, fileName);
            Assert.False(created.HasError, string.Join(", ", created.Errors));
            Assert.True(writedFile);

            var updated = await created.Move(updateName);
            Assert.False(created.HasError, string.Join(", ", created.Errors));
            this.AssertNodeSubFolder(updated);

            // Always fail if exists file.
            Assert.False(await updated.Delete());
            Assert.True(updated.HasError);

            updated.ClearErrors();
            var fileDeleted = await updated.Delete(fileName);
            Assert.False(created.HasError, string.Join(", ", created.Errors));
            Assert.True(fileDeleted);

            var folderDeleted = await updated.Delete();
            Assert.False(updated.HasError, string.Join(", ", updated.Errors));
            Assert.True(folderDeleted);
        }

        [Fact]
        public async Task ReadFailTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    this.AssertErrorResult(await node.Read(), node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    this.AssertErrorResult(await node.Read(), node);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    this.AssertErrorResult(await node.Read(), node);
                }
            }
        }

        [Fact]
        public async Task WriteFailTest()
        {
            var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    this.AssertErrorResult(await node.Write(stream), node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    this.AssertErrorResult(await node.Write(stream), node);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    this.AssertErrorResult(await node.Write(stream), node);
                }
            }
        }
    }
}
