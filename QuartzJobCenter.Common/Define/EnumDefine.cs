using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzJobCenter.Common.Define
{
    public class EnumDefine
    {
        public enum MailMessageEnum
        {
            None = 0,
            Err = 1,
            All = 2
        }

        public enum ScheduleTypeEnum
        {
            Http = 0,
            GRPC = 1
        }

        public enum RequestTypeEnum
        {
            None = 0,
            Get = 1,
            Post = 2,
            Put = 4,
            Delete = 8
        }

        public enum TriggerTypeEnum
        {
            None = 0,
            Cron = 1,
            Simple = 2,
        }

        public enum ResponseCodeEnum
        {
            Success = 0,
            Fail = 400,
            Error = 500
        }
    }
}
