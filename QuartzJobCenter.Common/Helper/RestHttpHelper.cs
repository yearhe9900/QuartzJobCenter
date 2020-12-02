using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace QuartzJobCenter.Common.Helper
{

    public class RestHttpHelper
    {
        private static readonly Lazy<RestHttpHelper> lazy = new Lazy<RestHttpHelper>(() => new RestHttpHelper());

        public static RestHttpHelper Instance { get { return lazy.Value; } }

        private RestHttpHelper() { }

        /// <summary>
        /// 获取已保存的站点http客户端
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static (RestClient restClient, Uri uri) GetRestClient(string url)
        {
            /// <summary>
            /// 不同url分配不同RestClient
            /// </summary>
            Dictionary<string, RestClient> dictionary = new Dictionary<string, RestClient>();
            var uri = new Uri(url);
            var key = uri.Scheme + uri.Host;
            if (!dictionary.Keys.Contains(key))
                dictionary.Add(key, new RestClient(uri.Scheme + "://" + uri.Host));
            return (dictionary[key], uri);
        }

        #region GET请求

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="headers">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers = null)
        {
            var (client, uri) = GetRestClient(url);
            var localPath = uri.LocalPath;
            var query = uri.Query.Substring(1);
            var request = new RestRequest(localPath, Method.GET);//创建一个GET请求
            AddQueryParameters(request, query);
            return null;
        }

        #endregion

        #region 私有方法 

        public static void AddQueryParameters(RestRequest restRequest, string query)
        {
            var paramArray = query.Split('&');
            foreach (var paramStr in paramArray)
            {
                var paramItems = paramStr.Split('=');
                restRequest.AddParameter(paramItems[0], paramItems[1]);
            }
        }

        #endregion
    }
}
