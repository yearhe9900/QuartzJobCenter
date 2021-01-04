using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuartzJobCenter.Common.Helper;

namespace QuartzJobCenter.Common.Test
{
    [TestClass]
    public class RestHttpHelperTest
    {
        [TestMethod]
        public void PostAsync_Test()
        {
            var http = RestHttpHelper.Instance;
            var result = http.PostAsync("http://192.168.137.253:7300/mock/5feaf7dad8b2db0021a0e9bc/mock_beite/Api/Meter/CommandState").Result;
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.OK);
        }
    }
}
