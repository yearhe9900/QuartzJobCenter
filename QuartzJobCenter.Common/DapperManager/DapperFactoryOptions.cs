using System;
using System.Collections.Generic;

namespace QuartzJobCenter.Common.DapperManager
{
    public class DapperFactoryOptions
    {
        public IList<Action<ConnectionConfig>> DapperActions { get; } = new List<Action<ConnectionConfig>>();
    }
}
