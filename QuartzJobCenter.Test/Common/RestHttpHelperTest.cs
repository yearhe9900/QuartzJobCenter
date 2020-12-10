using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuartzJobCenter.Common.Helper;

namespace QuartzJobCenter.Common.Test
{
    [TestClass]
    public class RestHttpHelperTest
    {
        [TestMethod]
        public void GetAsync_Test()
        {
            var http = RestHttpHelper.Instance;
            var result = http.GetAsync("https://www.baidu.com").Result;
            var aa = result.Content;
        }
    }
}
