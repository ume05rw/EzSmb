using EzSmb;
using EzSmb.Params;
using EzSmb.Params.Enums;
using EzSmbTest.Bases;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EzSmbTest
{
    public class NodeStaticTest : TestBase
    {
        public NodeStaticTest() : base()
        {
        }

        [Fact]
        public async Task GetNodeTestServerByIpAddress()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.Address}";
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeServer(node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeServer(node);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeServer(node);
                }
            }
        }

        [Fact]
        public async Task GetNodeTestGetServerByHostName()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.HostName}";
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeServer(node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeServer(node);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeServer(node);
                }
            }
        }

        [Fact]
        public async Task GetNodeTestShareFolderByIpAddress()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.Address}/{setting.TestPath.Share}";
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeShareFolder(node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeShareFolder(node);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeShareFolder(node);
                }
            }
        }

        [Fact]
        public async Task GetNodeTestShareFolderByHostName()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.HostName}/{setting.TestPath.Share}";
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeShareFolder(node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeShareFolder(node);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeShareFolder(node);
                }
            }
        }

        [Fact]
        public async Task GetNodeTestSubFolderByIpAddress()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.Address}/{setting.TestPath.Folder}";
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeSubFolder(node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeSubFolder(node);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeSubFolder(node);
                }
            }
        }

        [Fact]
        public async Task GetNodeTestSubFolderByHostName()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.HostName}/{setting.TestPath.Folder}";
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeSubFolder(node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeSubFolder(node);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeSubFolder(node);
                }
            }
        }

        [Fact]
        public async Task GetNodeTestFileByIpAddress()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.Address}/{setting.TestPath.File}";
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeFile(node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeFile(node);
                }


                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeFile(node);
                }
            }
        }

        [Fact]
        public async Task GetNodeTestFileByHostName()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.HostName}/{setting.TestPath.File}";
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeFile(node);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeFile(node);
                }


                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeFile(node);
                }
            }
        }

        [Fact]
        public async Task GetNodeTestRelatedServer()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var relSet = setting.TestPath.RelatedServer;
                var path = $"{setting.Address}/{relSet.Path}";
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeServer(node);
                    Assert.Equal(relSet.Name, node.Name);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeServer(node);
                    Assert.Equal(relSet.Name, node.Name);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeServer(node);
                    Assert.Equal(relSet.Name, node.Name);
                }
            }
        }

        [Fact]
        public async Task GetNodeTestRelatedShareFolder()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var relSet = setting.TestPath.RelatedShare;
                var path = $"{setting.Address}/{relSet.Path}";
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeShareFolder(node);
                    Assert.Equal(relSet.Name, node.Name);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeShareFolder(node);
                    Assert.Equal(relSet.Name, node.Name);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeShareFolder(node);
                    Assert.Equal(relSet.Name, node.Name);
                }
            }
        }

        [Fact]
        public async Task GetNodeTestRelatedSubFolder()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var relSet = setting.TestPath.RelatedFolder;
                var path = $"{setting.Address}/{relSet.Path}";
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeSubFolder(node);
                    Assert.Equal(relSet.Name, node.Name);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeSubFolder(node);
                    Assert.Equal(relSet.Name, node.Name);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeSubFolder(node);
                    Assert.Equal(relSet.Name, node.Name);
                }
            }
        }

        [Fact]
        public async Task GetNodeTestRelatedFile()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var relSet = setting.TestPath.RelatedFolder;
                var path = $"{setting.Address}/{relSet.Path}";
                Node node;

                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                {
                    set.SmbType = null;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeSubFolder(node);
                    Assert.Equal(relSet.Name, node.Name);
                }

                if (setting.SupportedSmb1)
                {
                    set.SmbType = SmbType.Smb1;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeSubFolder(node);
                    Assert.Equal(relSet.Name, node.Name);
                }

                if (setting.SupportedSmb2)
                {
                    set.SmbType = SmbType.Smb2;
                    node = await Node.GetNode(path, set);
                    this.AssertNodeSubFolder(node);
                    Assert.Equal(relSet.Name, node.Name);
                }
            }
        }

        ///// <summary>
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks>
        ///// time too long...
        ///// </remarks>
        //[Fact]
        //public async Task GetNodeTestServerFail()
        //{
        //    foreach (var setting in this.Settings)
        //    {
        //        var set = this.GetParamSet(setting);
        //        var path = $"192.168.1.234";

        //        set.SmbType = null;
        //        if (setting.SupportedSmb1 || setting.SupportedSmb2)
        //            await this.AssertGetNodeFail(path, set);

        //        set.SmbType = SmbType.Smb1;
        //        if (setting.SupportedSmb1)
        //            await this.AssertGetNodeFail(path, set);

        //        set.SmbType = SmbType.Smb2;
        //        if (setting.SupportedSmb2)
        //            await this.AssertGetNodeFail(path, set);
        //    }
        //}

        [Fact]
        public async Task GetNodeTestShareFail()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.Address}/{setting.TestPath.FailShare}";

                set.SmbType = null;
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    await this.AssertGetNodeFail(path, set);

                set.SmbType = SmbType.Smb1;
                if (setting.SupportedSmb1)
                    await this.AssertGetNodeFail(path, set);

                set.SmbType = SmbType.Smb2;
                if (setting.SupportedSmb2)
                    await this.AssertGetNodeFail(path, set);
            }
        }

        [Fact]
        public async Task GetNodeTestSubFolderFail()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.Address}/{setting.TestPath.FailFolder}";

                set.SmbType = null;
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    await this.AssertGetNodeFail(path, set);

                set.SmbType = SmbType.Smb1;
                if (setting.SupportedSmb1)
                    await this.AssertGetNodeFail(path, set);

                set.SmbType = SmbType.Smb2;
                if (setting.SupportedSmb2)
                    await this.AssertGetNodeFail(path, set);
            }
        }

        [Fact]
        public async Task GetNodeTestFileFail()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.Address}/{setting.TestPath.FailFile}";

                set.SmbType = null;
                if (setting.SupportedSmb1 || setting.SupportedSmb2)
                    await this.AssertGetNodeFail(path, set);

                set.SmbType = SmbType.Smb1;
                if (setting.SupportedSmb1)
                    await this.AssertGetNodeFail(path, set);

                set.SmbType = SmbType.Smb2;
                if (setting.SupportedSmb2)
                    await this.AssertGetNodeFail(path, set);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "<保留中>")]
        private async Task AssertGetNodeFail(string path, ParamSet set)
        {
            Node node = null;
            try
            {
                node = await Node.GetNode(path, set, true);
                Assert.True(false, "Not throw exception.");
            }
            catch (Exception)
            {
            }
            Assert.Null(node);

            node = await Node.GetNode(path, set, false);
            Assert.Null(node);
        }

        [Fact]
        public async Task GetServersTest()
        {
            var result1 = await Node.GetServers();
            Assert.NotNull(result1);
            foreach (var addr in result1)
            {
                Assert.False(string.IsNullOrEmpty(addr));
            }

            var result2 = await Node.GetServers(5000);
            Assert.NotNull(result2);
            foreach (var addr in result2)
            {
                Assert.False(string.IsNullOrEmpty(addr));
            }
        }
    }
}
