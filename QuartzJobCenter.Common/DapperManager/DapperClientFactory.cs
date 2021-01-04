using System;
using Microsoft.Extensions.Options;
using QuartzJobCenter.Models.Define;

namespace QuartzJobCenter.Common.DapperManager
{
    public class DapperClientFactory : IDapperClientFactory
    {
        private readonly IOptionsMonitor<DapperClientListOptions> _optionsMonitorClient;
        public DapperClientFactory(IOptionsMonitor<DapperClientListOptions> optionsMonitorClient)
        {
            _optionsMonitorClient = optionsMonitorClient ?? throw new ArgumentNullException(nameof(optionsMonitorClient));
        }

        public DapperClient GetClient(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var clients = _optionsMonitorClient.Get(ConstantDefine.DbConnedtions).DapperDictionary;
            DapperClient client = null;
            foreach (var (key, value) in clients)
            {
                if (key == name)
                {
                    client = value;
                    break;
                }
            }
            return client;
        }

        public DapperClient GetTaskCenterConnection() => GetClient(ConstantDefine.TaskCenterConnection);
    }
}
