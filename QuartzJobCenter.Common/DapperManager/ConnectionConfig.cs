using QuartzJobCenter.Models.Enums;

namespace QuartzJobCenter.Common.DapperManager
{
    public class ConnectionConfig
    {
        public string ConnectionString { get; set; }

        public EnumDbStoreType DbType { get; set; }
    }
}
