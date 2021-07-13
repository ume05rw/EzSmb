using EzSmb;
using EzSmb.Params.Enums;
using EzSmbTest.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EzSmbTest.Paths
{
    public class PathSetTest : TestBase
    {
        [Fact]
        public async Task CloneServerNodeTest()
        {
            foreach (var setting in this.Settings)
            {
                var set = this.GetParamSet(setting);
                var path = $"{setting.HostName}";
                //var path = $"{setting.HostName}/{setting.TestPath.Share.Path}";

                set.SmbType = SmbType.Smb2;
                var node = await Node.GetNode(path, set);

                var clone = node.PathSet.Clone();
                Assert.False(Object.ReferenceEquals(node.PathSet, clone));

                Assert.Equal(node.PathSet.IpAddressString, clone.IpAddressString);

                Assert.Equal(node.PathSet.IpAddressString, clone.IpAddressString);
                Assert.False(Object.ReferenceEquals(node.PathSet.IpAddressString, clone.IpAddressString));

                Assert.Equal(node.PathSet.IpAddress.ToString(), clone.IpAddress.ToString());
                Assert.False(Object.ReferenceEquals(node.PathSet.IpAddress, clone.IpAddress));

                Assert.Equal(node.PathSet.Share, clone.Share);
                if (node.PathSet.Share == null)
                {
                    Assert.Null(clone.Share);
                }
                else
                {
                    Assert.False(Object.ReferenceEquals(node.PathSet.Share, clone.Share));
                }

                Assert.Equal(node.PathSet.ElementsPath, clone.ElementsPath);
                if (node.PathSet.ElementsPath == null)
                {
                    Assert.Null(clone.ElementsPath);
                }
                else
                {
                    Assert.False(Object.ReferenceEquals(node.PathSet.ElementsPath, clone.ElementsPath));
                }


                Assert.Equal(node.PathSet.FullPath, clone.FullPath);
                Assert.False(Object.ReferenceEquals(node.PathSet.FullPath, clone.FullPath));

                if (node.PathSet.Elements == null)
                {
                    Assert.Null(clone.Elements);
                }
                else
                {
                    Assert.NotNull(clone.Elements);
                    Assert.Equal(node.PathSet.Elements.Length, clone.Elements.Length);
                    Assert.False(Object.ReferenceEquals(node.PathSet.Elements, clone.Elements));

                    for (var i = 0; i < node.PathSet.Elements.Length; i++)
                    {
                        Assert.Equal(node.PathSet.Elements[i], clone.Elements[i]);
                        Assert.False(Object.ReferenceEquals(node.PathSet.Elements[i], clone.Elements[i]));
                    }
                }
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

                var clone = node.PathSet.Clone();
                Assert.False(Object.ReferenceEquals(node.PathSet, clone));

                Assert.Equal(node.PathSet.IpAddressString, clone.IpAddressString);

                Assert.Equal(node.PathSet.IpAddressString, clone.IpAddressString);
                Assert.False(Object.ReferenceEquals(node.PathSet.IpAddressString, clone.IpAddressString));

                Assert.Equal(node.PathSet.IpAddress.ToString(), clone.IpAddress.ToString());
                Assert.False(Object.ReferenceEquals(node.PathSet.IpAddress, clone.IpAddress));

                Assert.Equal(node.PathSet.Share, clone.Share);
                if (node.PathSet.Share == null)
                {
                    Assert.Null(clone.Share);
                }
                else
                {
                    Assert.False(Object.ReferenceEquals(node.PathSet.Share, clone.Share));
                }

                Assert.Equal(node.PathSet.ElementsPath, clone.ElementsPath);
                if (node.PathSet.ElementsPath == null)
                {
                    Assert.Null(clone.ElementsPath);
                }
                else
                {
                    Assert.False(Object.ReferenceEquals(node.PathSet.ElementsPath, clone.ElementsPath));
                }


                Assert.Equal(node.PathSet.FullPath, clone.FullPath);
                Assert.False(Object.ReferenceEquals(node.PathSet.FullPath, clone.FullPath));

                if (node.PathSet.Elements == null)
                {
                    Assert.Null(clone.Elements);
                }
                else
                {
                    Assert.NotNull(clone.Elements);
                    Assert.Equal(node.PathSet.Elements.Length, clone.Elements.Length);
                    Assert.False(Object.ReferenceEquals(node.PathSet.Elements, clone.Elements));

                    for (var i = 0; i < node.PathSet.Elements.Length; i++)
                    {
                        Assert.Equal(node.PathSet.Elements[i], clone.Elements[i]);
                        Assert.False(Object.ReferenceEquals(node.PathSet.Elements[i], clone.Elements[i]));
                    }
                }
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

                var clone = node.PathSet.Clone();
                Assert.False(Object.ReferenceEquals(node.PathSet, clone));

                Assert.Equal(node.PathSet.IpAddressString, clone.IpAddressString);

                Assert.Equal(node.PathSet.IpAddressString, clone.IpAddressString);
                Assert.False(Object.ReferenceEquals(node.PathSet.IpAddressString, clone.IpAddressString));

                Assert.Equal(node.PathSet.IpAddress.ToString(), clone.IpAddress.ToString());
                Assert.False(Object.ReferenceEquals(node.PathSet.IpAddress, clone.IpAddress));

                Assert.Equal(node.PathSet.Share, clone.Share);
                if (node.PathSet.Share == null)
                {
                    Assert.Null(clone.Share);
                }
                else
                {
                    Assert.False(Object.ReferenceEquals(node.PathSet.Share, clone.Share));
                }

                Assert.Equal(node.PathSet.ElementsPath, clone.ElementsPath);
                if (node.PathSet.ElementsPath == null)
                {
                    Assert.Null(clone.ElementsPath);
                }
                else
                {
                    Assert.False(Object.ReferenceEquals(node.PathSet.ElementsPath, clone.ElementsPath));
                }


                Assert.Equal(node.PathSet.FullPath, clone.FullPath);
                Assert.False(Object.ReferenceEquals(node.PathSet.FullPath, clone.FullPath));

                if (node.PathSet.Elements == null)
                {
                    Assert.Null(clone.Elements);
                }
                else
                {
                    Assert.NotNull(clone.Elements);
                    Assert.Equal(node.PathSet.Elements.Length, clone.Elements.Length);
                    Assert.False(Object.ReferenceEquals(node.PathSet.Elements, clone.Elements));

                    for (var i = 0; i < node.PathSet.Elements.Length; i++)
                    {
                        Assert.Equal(node.PathSet.Elements[i], clone.Elements[i]);
                        Assert.False(Object.ReferenceEquals(node.PathSet.Elements[i], clone.Elements[i]));
                    }
                }
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

                var clone = node.PathSet.Clone();
                Assert.False(Object.ReferenceEquals(node.PathSet, clone));

                Assert.Equal(node.PathSet.IpAddressString, clone.IpAddressString);

                Assert.Equal(node.PathSet.IpAddressString, clone.IpAddressString);
                Assert.False(Object.ReferenceEquals(node.PathSet.IpAddressString, clone.IpAddressString));

                Assert.Equal(node.PathSet.IpAddress.ToString(), clone.IpAddress.ToString());
                Assert.False(Object.ReferenceEquals(node.PathSet.IpAddress, clone.IpAddress));

                Assert.Equal(node.PathSet.Share, clone.Share);
                if (node.PathSet.Share == null)
                {
                    Assert.Null(clone.Share);
                }
                else
                {
                    Assert.False(Object.ReferenceEquals(node.PathSet.Share, clone.Share));
                }

                Assert.Equal(node.PathSet.ElementsPath, clone.ElementsPath);
                if (node.PathSet.ElementsPath == null)
                {
                    Assert.Null(clone.ElementsPath);
                }
                else
                {
                    Assert.False(Object.ReferenceEquals(node.PathSet.ElementsPath, clone.ElementsPath));
                }


                Assert.Equal(node.PathSet.FullPath, clone.FullPath);
                Assert.False(Object.ReferenceEquals(node.PathSet.FullPath, clone.FullPath));

                if (node.PathSet.Elements == null)
                {
                    Assert.Null(clone.Elements);
                }
                else
                {
                    Assert.NotNull(clone.Elements);
                    Assert.Equal(node.PathSet.Elements.Length, clone.Elements.Length);
                    Assert.False(Object.ReferenceEquals(node.PathSet.Elements, clone.Elements));

                    for (var i = 0; i < node.PathSet.Elements.Length; i++)
                    {
                        Assert.Equal(node.PathSet.Elements[i], clone.Elements[i]);
                        Assert.False(Object.ReferenceEquals(node.PathSet.Elements[i], clone.Elements[i]));
                    }
                }
            }
        }
    }
}
