using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using RestSharp;
using static QuartzJobCenter.Common.JobTask;

namespace QuartzJobCenter.Common.Helper
{

    public class GrpcClientHelper
    {
        private static readonly Lazy<GrpcClientHelper> lazy = new Lazy<GrpcClientHelper>(() => new GrpcClientHelper());

        public static GrpcClientHelper Instance { get { return lazy.Value; } }

        private GrpcClientHelper() { }

        /// <summary>
        /// 获取已保存的站点grpc客户端
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static JobTaskClient GetJobTaskClient(string url)
        {
            /// <summary>
            /// 不同url分配不同JobTaskClient
            /// </summary>
            Dictionary<string, JobTaskClient> dictionary = new Dictionary<string, JobTaskClient>();
            if (!dictionary.Keys.Contains(url))
            {
                var channel = GrpcChannel.ForAddress(url);
                var greeterClient = new JobTaskClient(channel);
                dictionary.Add(url, greeterClient);
            }
            return dictionary[url];
        }

        #region Excute

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="headers">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<ExcuteReply> ExcuteAsync(string url, object paramsObj)
        {
            var client = GetJobTaskClient(url);
            return await client.ExcuteAsync(new ExcuteRequest()
            {
                Params = Newtonsoft.Json.JsonConvert.SerializeObject(paramsObj)
            });
        }

        #endregion

    }
}
