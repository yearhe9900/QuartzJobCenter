using System;
using static QuartzJobCenter.Common.Define.EnumDefine;

namespace QuartzJobCenter.Models.Entities
{
    public class ScheduleEntity
    {
        public string SchedulerName { get; set; }

        /// <summary>
        /// 是否编辑
        /// </summary>
        public string IsEdit { get; set; }

        /// <summary>
        /// 任务分组(编辑用)
        /// </summary>
        public string OldGroupName { get; set; }

        /// <summary>
        /// 任务名称(编辑用)
        /// </summary>
        public string OldName { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// 任务分组
        /// </summary>
        public string JobGroup { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Cron表达式
        /// </summary>
        public string Cron { get; set; }

        /// <summary>
        /// 执行次数（默认无限循环）
        /// </summary>
        public int? RunTimes { get; set; }

        /// <summary>
        /// 执行间隔时间，单位秒（如果有Cron，则IntervalSecond失效）
        /// </summary>
        public int? IntervalSecond { get; set; }

        /// <summary>
        /// 调度方式
        /// </summary>
        public ScheduleTypeEnum ScheduleType { get; set; }

        /// <summary>
        /// 触发器类型
        /// </summary>
        public TriggerTypeEnum TriggerType { get; set; } = TriggerTypeEnum.Cron;

        /// <summary>
        /// 请求url
        /// </summary>
        public string RequestUrl { get; set; }

        /// <summary>
        /// 请求参数（Post，Put请求用）
        /// </summary>
        public string RequestParameters { get; set; }

        /// <summary>
        /// Headers(可以包含如：Authorization授权认证)
        /// 格式：{"Authorization":"userpassword.."}
        /// </summary>
        public string Headers { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        public RequestTypeEnum RequestType { get; set; } = RequestTypeEnum.Post;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public MailMessageEnum MailMessage { get; set; } = MailMessageEnum.None;
    }
}
