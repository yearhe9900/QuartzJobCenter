using Quartz;
using QuartzJobCenter.Common.Define;
using QuartzJobCenter.Models.Entities;
using QuartzJobCenter.Models.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static QuartzJobCenter.Common.Define.EnumDefine;

namespace QuartzJobCenter.Host.Components
{
    public class SchedulerCenter
    {
        private readonly IScheduler _scheduler;

        /// <summary>
        /// 任务调度对象
        /// </summary>
        public static readonly Lazy<SchedulerCenter> _lazy = new Lazy<SchedulerCenter>(() => new SchedulerCenter());

        public static SchedulerCenter Instance { get { return _lazy.Value; } }

        /// <summary>
        /// 添加调度任务
        /// </summary>
        /// <param name="scheduleEntity"></param>
        /// <returns></returns>
        public async Task<BaseResultResponse> AddScheduleJobAsync(ScheduleEntity entity)
        {
            var result = new BaseResultResponse();
            try
            {
                //检查任务是否已存在
                var jobKey = new JobKey(entity.JobName, entity.JobGroup);
                if (await _scheduler.CheckExists(jobKey))
                {
                    result.Code = (int)ResponseCodeEnum.Error;
                    result.Msg = "任务已存在";
                    return result;
                }
                if (entity.ScheduleType == ScheduleTypeEnum.Http)
                {
                    //http请求配置
                    var httpDir = new Dictionary<string, string>()
                {
                    { ConstantDefine.REQUESTURL,entity.RequestUrl},
                    { ConstantDefine.REQUESTPARAMETERS,entity.RequestParameters},
                    { ConstantDefine.REQUESTTYPE, ((int)entity.RequestType).ToString()},
                    { ConstantDefine.HEADERS, entity.Headers},
                    { ConstantDefine.MAILMESSAGE, ((int)entity.MailMessage).ToString()},
                };
                    // 定义这个工作，并将其绑定到我们的IJob实现类                
                    IJobDetail job = JobBuilder.Create<HttpJob>()
                        .SetJobData(new JobDataMap(httpDir))
                        .WithDescription(entity.Description)
                        .WithIdentity(entity.JobName, entity.JobGroup)
                        .Build();

                    // 创建触发器
                    ITrigger trigger;
                    trigger = CreateTrigger(entity);

                    // 告诉Quartz使用我们的触发器来安排作业
                    await _scheduler.ScheduleJob(job, trigger);
                    result.Code = 200;
                }
            }
            catch (Exception ex)
            {
                result.Code = (int)ResponseCodeEnum.Error;
                result.Msg = ex.ToString();
            }
            return result;
        }


        /// <summary>
        /// 创建类型Cron的触发器
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private ITrigger CreateTrigger(ScheduleEntity entity)
        {
            var trigger = TriggerBuilder.Create().WithIdentity(entity.JobName, entity.JobGroup);

            if (entity.BeginTime.HasValue && entity.BeginTime.Value != DateTime.MinValue)
            {
                trigger.StartAt(entity.BeginTime.Value);
            }

            if (entity.EndTime.HasValue && entity.EndTime.Value != DateTime.MinValue)
            {
                trigger.EndAt(entity.EndTime.Value);
            }

            if (entity.TriggerType == TriggerTypeEnum.Cron)
            {
                trigger.WithCronSchedule(entity.Cron, cronScheduleBuilder => cronScheduleBuilder.WithMisfireHandlingInstructionFireAndProceed());//指定cron表达式
            }
            else
            {
                if (entity.RunTimes.HasValue && entity.RunTimes > 0)
                {
                    trigger.WithSimpleSchedule(x =>
                    {
                        x.WithIntervalInSeconds(entity.IntervalSecond.Value)//执行时间间隔，单位秒
                        .WithRepeatCount(entity.RunTimes.Value)//执行次数、默认从0开始
                        .WithMisfireHandlingInstructionFireNow();
                    });
                }
                else
                {
                    trigger.WithSimpleSchedule(x =>
                    {
                        x.WithIntervalInSeconds(entity.IntervalSecond.Value)//执行时间间隔，单位秒
                        .RepeatForever()//无限循环
                        .WithMisfireHandlingInstructionFireNow();
                    });
                }
            }

            return trigger.ForJob(entity.JobName, entity.JobGroup)//作业名称
                .Build();
        }
    }
}
