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
            var result = http.GetAsync("https://www.baidu.com/s?ie=utf-8&csq=1&pstg=20&mod=2&isbd=1&cqid=d9dd16d1001a710f&istc=635&ver=QNAbSAIJ0uLajezqpKPUmu9Z20ZrZSuNCIG&chk=5fc74980&isid=BE75CE9290E28940&ie=utf-8&f=8&rsv_bp=1&tn=baidu&wd=HttpClient%20RestSharp%E5%AF%B9%E6%AF%94&oq=Http%2526lt%253Blient%2520RestSharp%25E5%25AF%25B9%25E6%25AF%2594&rsv_pq=e296096100103f63&rsv_t=9f54NEO1QpuZSpSMQjrHEPlbXK7I3aIz94KoXeXOKMei40QIqnwbFZPJ6wg&rqlang=cn&rsv_dl=tb&rsv_enter=0&rsv_btype=t&bs=HttpClient%20RestSharp%E5%AF%B9%E6%AF%94&f4s=1&_ck=54040.1.120.79.29.667.44&isnop=0&rsv_stat=-2&rsv_bp=1").Result;
        }
    }
}
