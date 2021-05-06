using SmbClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmbClientTest
{
    public class UtilsTest
    {
        [Fact]
        public void ResolveTest()
        {
            var resolved = Utils.Resolve("192.168.0.1/Share/test/../../Share2/aaa/../test.txt");
            Assert.Equal(@"192.168.0.1\Share2\test.txt", resolved);
        }
    }
}
