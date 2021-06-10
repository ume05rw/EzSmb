using EzSmb;
using EzSmb.Params;
using EzSmb.Params.Enums;
using EzSmbTest.Bases;
using EzSmbTest.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EzSmbTest
{
    public class SharedFolderNodeTest : TestBase
    {
        public SharedFolderNodeTest() : base()
        {
        }

        /// <summary>
        /// このファイルで取得するNodeは全て、サーバ直下の共有フォルダ
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        private async Task<Node> GetNode(Setting setting, ParamSet set)
        {
            var path = $@"{setting.Address}\{setting.TestPath.Share.Path}";
            var node = await Node.GetNode(path, set);

            if (set.SmbType == null)
            {
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    this.AssertNodeShareFolder(node);
                else
                    Assert.Null(node);
            }
            else if (set.SmbType == SmbType.Smb1)
            {
                if (setting.SupportedSmb1)
                    this.AssertNodeShareFolder(node);
                else
                    Assert.Null(node);
            }
            else if (set.SmbType == SmbType.Smb2)
            {
                if (setting.SupportedSmb2)
                    this.AssertNodeShareFolder(node);
                else
                    Assert.Null(node);
            }

            return node;
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
                    await this.AssertNodeItem(
                        await this.GetNode(setting, set),
                        setting.TestPath.Share
                    );
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    await this.AssertNodeItem(
                        await this.GetNode(setting, set),
                        setting.TestPath.Share
                    );
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    await this.AssertNodeItem(
                        await this.GetNode(setting, set),
                        setting.TestPath.Share
                    );
                }
            }
        }

        [Fact]
        public async Task GetNodeTestRelatedServer()
        {
            foreach (var setting in this.Settings)
            {
                var relatedPath = setting.TestPath.Share.RelatedServer;
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
                var relatedPath = setting.TestPath.Share.RelatedShare;
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
                var relatedPath = setting.TestPath.Share.RelatedFolder;
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
                var relatedPath = setting.TestPath.Share.RelatedFile;
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
        public async Task CreateFolderAndDeleteTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    await this.InnerCreateFolderAndDeleteTest(node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    await this.InnerCreateFolderAndDeleteTest(node);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    await this.InnerCreateFolderAndDeleteTest(node);
                }
            }
        }

        private async Task InnerCreateFolderAndDeleteTest(Node node)
        {
            var folderName = "created";

            var result = await node.CreateFolder(folderName);
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            this.AssertNodeSubFolder(result);

            var deleted = await result.Delete();
            Assert.False(result.HasError, string.Join(", ", result.Errors));
            Assert.True(deleted);
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

        [Fact]
        public async Task DeleteFailTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await this.GetNode(setting, set);
                    this.AssertErrorResult(await node.Delete(), node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await this.GetNode(setting, set);
                    this.AssertErrorResult(await node.Delete(), node);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    this.AssertErrorResult(await node.Delete(), node);
                }
            }
        }

        [Fact]
        public async Task MoveFailTest()
        {
            foreach (var setting in this.Settings)
            {
                var moveTo = setting.TestPath.FailShare.Path;
                var set = this.GetParamSet(setting);
                Node node;

                // SMB1 Not Supported.
                //if (setting.SupportedSmb1 || setting.SupportedSmb2)
                //{
                //    set.SmbType = null;
                //    node = await this.GetNode(setting, set);
                //    this.AssertErrorResult(await node.Move(moveTo), node);
                //}

                //if (setting.SupportedSmb1)
                //{
                //    set.SmbType = SmbType.Smb1;
                //    node = await this.GetNode(setting, set);
                //    this.AssertErrorResult(await node.Move(moveTo), node);
                //}

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await this.GetNode(setting, set);
                    this.AssertErrorResult(await node.Move(moveTo), node);
                }
            }
        }
    }
}
