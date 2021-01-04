using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuartzJobCenter.Common.Helper;

namespace QuartzJobCenter.Common.Test
{
    [TestClass]
    public class GrpcClientHelperTest
    {
        [TestMethod]
        public void ExcuteAsync_Test()
        {
            var http = GrpcClientHelper.Instance;
            var result = http.ExcuteAsync("https://localhost:5001", new { haha = "haha" }).Result;
            Assert.IsTrue(result.Status == Stauts.Success);
        }
    }
}
