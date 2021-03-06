﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
            var key = $"{uri.Scheme}://{uri.Authority}";
            if (!dictionary.Keys.Contains(key))
            {
                var client = new RestClient(key)
                {
                    Timeout = 6000//设置超时时间
                };
                dictionary.Add(key, client);
            }
            return (dictionary[key], uri);
        }

        #region GET请求

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="headers">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<IRestResponse> GetAsync(string url, Dictionary<string, string> headers = null)
        {
            var (client, uri) = GetRestClient(url);
            var localPath = uri.LocalPath;
            var query = string.IsNullOrWhiteSpace(uri.Query) ? uri.Query : uri.Query[1..];
            var request = new RestRequest(localPath, Method.GET);//创建一个GET请求
            AddQueryParameters(request, query);
            AddRequestHeader(request, headers);
            //CancellationTokenSource cts = new CancellationTokenSource();
            //cts.CancelAfter(10000);//请求超过指定时长后退出,避免长时间等待导致业务超时
            //return await client.ExecuteAsync(request, cts.Token);
            return await client.ExecuteAsync(request);
        }

        #endregion

        #region POST请求

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        public async Task<IRestResponse> PostAsync(string url, Dictionary<string, string> headers = null)
        {
            var (client, uri) = GetRestClient(url);
            var localPath = uri.LocalPath;
            var query = string.IsNullOrWhiteSpace(uri.Query) ? uri.Query : uri.Query[1..];
            var request = new RestRequest(localPath, Method.POST);//创建一个POST请求
            AddQueryParameters(request, query);
            AddRequestHeader(request, headers);
            return await client.ExecuteAsync(request);
        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        public async Task<IRestResponse> PostAsync(string url, object body, Dictionary<string, string> headers = null)
        {
            var (client, uri) = GetRestClient(url);
            var localPath = uri.LocalPath;
            var query = string.IsNullOrWhiteSpace(uri.Query) ? uri.Query : uri.Query[1..];
            var request = new RestRequest(localPath, Method.POST);//创建一个POST请求
            AddQueryParameters(request, query);
            AddRequestHeader(request, headers);
            if (body != null)
            {
                request.AddJsonBody(body);
            }
            return await client.ExecuteAsync(request);
        }

        #endregion

        #region 私有方法 

        /// <summary>
        /// 请求添加参数
        /// </summary>
        /// <param name="restRequest"></param>
        /// <param name="query"></param>
        private void AddQueryParameters(RestRequest restRequest, string query)
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                var paramArray = query.Split('&');
                foreach (var paramStr in paramArray)
                {
                    var paramItems = paramStr.Split('=');
                    restRequest.AddParameter(paramItems[0], paramItems[1]);
                }
            }
        }

        /// <summary>
        /// 请求添加头信息
        /// </summary>
        /// <param name="restRequest"></param>
        /// <param name="query"></param>
        private void AddRequestHeader(RestRequest restRequest, Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    restRequest.AddHeader(header.Key, header.Value);
                }
            }
        }

        #endregion
    }
}
