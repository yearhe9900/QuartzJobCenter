using static QuartzJobCenter.Models.Enums.EnumDefine;

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
