using System;
using System.Collections;
using System.Collections.Generic;

namespace QuartzJobCenter.Common.DapperManager
{
    public class DapperClientListOptions
    {
        public Dictionary<string, DapperClient> DapperDictionary { get; set; } =new Dictionary<string, DapperClient>();
    }
}
