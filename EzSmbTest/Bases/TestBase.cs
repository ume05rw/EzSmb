using EzSmb;
using EzSmb.Params;
using Newtonsoft.Json;
using EzSmbTest.Models;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace EzSmbTest.Bases
{
    public abstract class TestBase
    {
        private const string SettingsName = "settings.json";
        protected readonly Setting[] Settings;

        public TestBase()
        {
            if (!File.Exists(TestBase.SettingsName))
                throw new InvalidOperationException("settings.json Not Found.");

            var bytes = File.ReadAllBytes(TestBase.SettingsName);
            var json = Encoding.UTF8.GetString(bytes);
            this.Settings = JsonConvert.DeserializeObject<Setting[]>(json);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "<保留中>")]
        protected ParamSet GetParamSet(Setting setting)
        {
            return new ParamSet()
            {
                UserName = setting.UserName,
                Password = setting.Password,
                DomainName = setting.DomainName,
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "<保留中>")]
        private void AssertNode(Node node)
        {
            Assert.NotNull(node);
            Assert.False(string.IsNullOrEmpty(node.Name));
            Assert.False(string.IsNullOrEmpty(node.FullPath));
            Assert.NotNull(node.ParamSet);
            Assert.True(node.ParamSet is FixedParamSet);
            Assert.NotNull(node.ParamSet.UserName);
            Assert.NotNull(node.ParamSet.DomainName);

            Assert.NotNull(node.PathSet);
            Assert.NotNull(node.PathSet.IpAddress);
            Assert.False(string.IsNullOrEmpty(node.PathSet.IpAddressString));
            Assert.NotNull(node.PathSet.Elements);
            Assert.False(string.IsNullOrEmpty(node.PathSet.FullPath));
        }

        protected void AssertNodeServer(Node node)
        {
            this.AssertNode(node);
            Assert.Equal(NodeType.Server, node.Type);
            Assert.Null(node.PathSet.Share);
            Assert.Null(node.PathSet.ElementsPath);
            Assert.Empty(node.PathSet.Elements);
            Assert.Null(node.GetParent());
        }

        private void AssertNodeFolder(Node node)
        {
            this.AssertNode(node);
            Assert.Equal(NodeType.Folder, node.Type);
            Assert.False(string.IsNullOrEmpty(node.PathSet.Share));
            Assert.NotNull(node.GetParent());
        }

        protected void AssertNodeShareFolder(Node node)
        {
            this.AssertNodeFolder(node);
            Assert.Null(node.PathSet.ElementsPath);
            Assert.Empty(node.PathSet.Elements);

            this.AssertNodeServer(node.GetParent());
        }

        protected void AssertNodeSubFolder(Node node)
        {
            this.AssertNodeFolder(node);
            Assert.False(string.IsNullOrEmpty(node.PathSet.ElementsPath));
            Assert.NotEmpty(node.PathSet.Elements);

            if (node.GetParent().PathSet.Elements.Length <= 0)
                this.AssertNodeShareFolder(node.GetParent());
            else
                this.AssertNodeSubFolder(node.GetParent());
        }

        protected void AssertNodeFile(Node node)
        {
            this.AssertNode(node);
            Assert.Equal(NodeType.File, node.Type);
            Assert.False(string.IsNullOrEmpty(node.PathSet.Share));
            Assert.False(string.IsNullOrEmpty(node.PathSet.ElementsPath));
            Assert.NotEmpty(node.PathSet.Elements);
            Assert.NotNull(node.GetParent());

            if (node.GetParent().PathSet.Elements.Length <= 0)
                this.AssertNodeShareFolder(node.GetParent());
            else
                this.AssertNodeSubFolder(node.GetParent());
        }

        protected async Task AssertGetList(Node node)
        {
            Assert.True(node.Type == NodeType.Server || node.Type == NodeType.Folder);

            var subNodes = await node.GetList();
            Assert.False(node.HasError, string.Join(", ", node.Errors));
            Assert.NotNull(subNodes);

            foreach (var subNode in subNodes)
                this.AssertNode(subNode);
        }

        protected async Task AssertRelatedServer(Node node, Related related)
        {
            var relNode = await node.GetNode(related.Path);
            this.AssertNodeServer(relNode);
            Assert.Equal(related.Name, relNode.Name);
        }

        protected async Task AssertRelatedShareFolder(Node node, Related related)
        {
            var relNode = await node.GetNode(related.Path);
            this.AssertNodeShareFolder(relNode);
            Assert.Equal(related.Name, relNode.Name);
        }

        protected async Task AssertRelatedSubFolder(Node node, Related related)
        {
            var relNode = await node.GetNode(related.Path);
            this.AssertNodeSubFolder(relNode);
            Assert.Equal(related.Name, relNode.Name);
        }

        protected async Task AssertRelatedFile(Node node, Related related)
        {
            var relNode = await node.GetNode(related.Path);
            this.AssertNodeFile(relNode);
            Assert.Equal(related.Name, relNode.Name);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "<保留中>")]
        protected void AssertErrorResult(dynamic result, Node node)
        {
            if (result != null && result != false)
                Assert.True(false, "Not Failed.");
            Assert.True(node.HasError);
            Assert.NotEmpty(node.Errors);
        }

        protected void Dump(string message, [CallerMemberName] string propertyName = "")
        {
            Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: {this.GetType()}.{propertyName}: {message}");
        }

        protected void Dump(Exception ex, [CallerMemberName] string propertyName = "")
        {
            this.Dump($"Exception Message = {ex.Message}, StackTrace = {ex.StackTrace}", propertyName);
        }
    }
}
