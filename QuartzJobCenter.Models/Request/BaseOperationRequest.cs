using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QuartzJobCenter.Common.Define.EnumDefine;

namespace QuartzJobCenter.Models.Request
{
    public class BaseOperationRequest
    {
        public OperationTypeEnum OperationType { get; set; }

        public string GroupName { get; set; }

        public string Name { get; set; }

        public ScheduleTypeEnum SchedulerType { get; set; }
    }
}
