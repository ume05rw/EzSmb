using EzSmb;
using EzSmb.Params.Enums;
using EzSmbTest.Bases;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EzSmbTest.Params
{
    public class FixedParamSetTest : TestBase
    {
        [Fact]
        public async Task CloneServerNodeTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.HostName}";

                set.SmbType = SmbType.Smb2;
                var node = await Node.GetNode(path, set);

                var clone = node.ParamSet.Clone();

                Assert.False(Object.ReferenceEquals(node.ParamSet, clone));

                Assert.Equal(node.ParamSet.UserName, clone.UserName);
                Assert.False(Object.ReferenceEquals(node.ParamSet.UserName, clone.UserName));

                Assert.Equal(node.ParamSet.DomainName, clone.DomainName);
                Assert.False(Object.ReferenceEquals(node.ParamSet.DomainName, clone.DomainName));

                Assert.Equal(node.ParamSet.SmbType, clone.SmbType);
                Assert.False(Object.ReferenceEquals(node.ParamSet.SmbType, clone.SmbType));
            }
        }

        [Fact]
        public async Task CloneShareFolderNodeTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.HostName}/{setting.TestPath.Share.Path}";

                set.SmbType = SmbType.Smb2;
                var node = await Node.GetNode(path, set);

                var clone = node.ParamSet.Clone();

                Assert.False(Object.ReferenceEquals(node.ParamSet, clone));

                Assert.Equal(node.ParamSet.UserName, clone.UserName);
                Assert.False(Object.ReferenceEquals(node.ParamSet.UserName, clone.UserName));

                Assert.Equal(node.ParamSet.DomainName, clone.DomainName);
                Assert.False(Object.ReferenceEquals(node.ParamSet.DomainName, clone.DomainName));

                Assert.Equal(node.ParamSet.SmbType, clone.SmbType);
                Assert.False(Object.ReferenceEquals(node.ParamSet.SmbType, clone.SmbType));
            }
        }

        [Fact]
        public async Task CloneSubFolderNodeTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.HostName}/{setting.TestPath.Folder.Path}";

                set.SmbType = SmbType.Smb2;
                var node = await Node.GetNode(path, set);

                var clone = node.ParamSet.Clone();

                Assert.False(Object.ReferenceEquals(node.ParamSet, clone));

                Assert.Equal(node.ParamSet.UserName, clone.UserName);
                Assert.False(Object.ReferenceEquals(node.ParamSet.UserName, clone.UserName));

                Assert.Equal(node.ParamSet.DomainName, clone.DomainName);
                Assert.False(Object.ReferenceEquals(node.ParamSet.DomainName, clone.DomainName));

                Assert.Equal(node.ParamSet.SmbType, clone.SmbType);
                Assert.False(Object.ReferenceEquals(node.ParamSet.SmbType, clone.SmbType));
            }
        }

        [Fact]
        public async Task CloneFileNodeTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.HostName}/{setting.TestPath.File.Path}";

                set.SmbType = SmbType.Smb2;
                var node = await Node.GetNode(path, set);

                var clone = node.ParamSet.Clone();
                Assert.False(Object.ReferenceEquals(node.ParamSet, clone));

                Assert.Equal(node.ParamSet.UserName, clone.UserName);
                Assert.False(Object.ReferenceEquals(node.ParamSet.UserName, clone.UserName));

                Assert.Equal(node.ParamSet.DomainName, clone.DomainName);
                Assert.False(Object.ReferenceEquals(node.ParamSet.DomainName, clone.DomainName));

                Assert.Equal(node.ParamSet.SmbType, clone.SmbType);
                Assert.False(Object.ReferenceEquals(node.ParamSet.SmbType, clone.SmbType));
            }
        }
    }
}
