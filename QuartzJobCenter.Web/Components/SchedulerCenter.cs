using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using Quartz.Util;
using QuartzJobCenter.Common.Define;
using QuartzJobCenter.Jobs;
using QuartzJobCenter.Models.Entities;
using QuartzJobCenter.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static QuartzJobCenter.Common.Define.EnumDefine;
using Quartz.Simpl;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore.Common;

namespace QuartzJobCenter.Web.Components
{
    public class SchedulerCenter
    {
        private IScheduler _scheduler;
        private IDbProvider _dbProvider;
        private string _driverDelegateType;

        /// <summary>
        /// 任务调度对象
        /// </summary>
        public static readonly Lazy<SchedulerCenter> _lazy = new Lazy<SchedulerCenter>(() => new SchedulerCenter());

        public static SchedulerCenter Instance { get { return _lazy.Value; } }

        /// <summary>
        /// 配置Scheduler 仅初始化时生效
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <param name="driverDelegateType"></param>
        public void Setting(IDbProvider dbProvider, string driverDelegateType)
        {
            _driverDelegateType = driverDelegateType;
            _dbProvider = dbProvider;
        }

        private IScheduler Scheduler
        {
            get
            {
                if (_scheduler != null)
                {
                    return _scheduler;
                }

                if (_dbProvider == null || string.IsNullOrEmpty(_driverDelegateType))
                {
                    throw new Exception("dbProvider or driverDelegateType is null");
                }

                DBConnectionManager.Instance.AddConnectionProvider("default", _dbProvider);
                var serializer = new JsonObjectSerializer();
                serializer.Initialize();
                var jobStore = new JobStoreTX
                {
                    DataSource = "default",
                    TablePrefix = "QRTZ_",
                    InstanceId = "AUTO",
                    DriverDelegateType = _driverDelegateType,
                    ObjectSerializer = serializer,
                };
                DirectSchedulerFactory.Instance.CreateScheduler("benny" + "Scheduler", "AUTO", new DefaultThreadPool(), jobStore);
                _scheduler = SchedulerRepository.Instance.Lookup("benny" + "Scheduler").Result;

                _scheduler.Start();//默认开始调度器
                return _scheduler;
            }
        }

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
                if (await Scheduler.CheckExists(jobKey))
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
                    await Scheduler.ScheduleJob(job, trigger);
                    result.Code = (int)ResponseCodeEnum.Success;
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
        /// 获取所有Job（详情信息 - 初始化页面调用）
        /// </summary>
        /// <returns></returns>
        public async Task<List<JobInfoEntity>> GetAllJobAsync()
        {
            List<JobKey> jboKeyList = new List<JobKey>();
            List<JobInfoEntity> jobInfoList = new List<JobInfoEntity>();
            if (Scheduler != null)
            {
                var groupNames = await Scheduler.GetJobGroupNames();
                foreach (var groupName in groupNames.OrderBy(t => t))
                {
                    jboKeyList.AddRange(await Scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)));
                    jobInfoList.Add(new JobInfoEntity() { GroupName = groupName });
                }
                foreach (var jobKey in jboKeyList.OrderBy(t => t.Name))
                {
                    var jobDetail = await Scheduler.GetJobDetail(jobKey);
                    var triggersList = await Scheduler.GetTriggersOfJob(jobKey);
                    var triggers = triggersList.AsEnumerable().FirstOrDefault();

                    var interval = string.Empty;
                    if (triggers is SimpleTriggerImpl)
                        interval = (triggers as SimpleTriggerImpl)?.RepeatInterval.ToString();
                    else
                        interval = (triggers as CronTriggerImpl)?.CronExpressionString;

                    foreach (var jobInfo in jobInfoList)
                    {
                        if (jobInfo.GroupName == jobKey.Group)
                        {
                            jobInfo.JobInfoList.Add(new JobInfo()
                            {
                                Name = jobKey.Name,
                                LastErrMsg = jobDetail.JobDataMap.GetString(ConstantDefine.EXCEPTION),
                                RequestUrl = jobDetail.JobDataMap.GetString(ConstantDefine.REQUESTURL),
                                TriggerState = await Scheduler.GetTriggerState(triggers.Key),
                                PreviousFireTime = triggers.GetPreviousFireTimeUtc()?.LocalDateTime,
                                NextFireTime = triggers.GetNextFireTimeUtc()?.LocalDateTime,
                                BeginTime = triggers.StartTimeUtc.LocalDateTime,
                                Interval = interval,
                                EndTime = triggers.EndTimeUtc?.LocalDateTime,
                                Description = jobDetail.Description
                            });
                            continue;
                        }
                    }
                }
            }
            return jobInfoList;
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
