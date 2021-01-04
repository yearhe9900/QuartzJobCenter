using Quartz;
using Quartz.Impl.Triggers;
using Quartz.Util;
using QuartzJobCenter.Models.Entities;
using QuartzJobCenter.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz.Simpl;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore.Common;
using QuartzJobCenter.Models.Request;
using QuartzJobCenter.Models.Options;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using QuartzJobCenter.Models.Define;
using static QuartzJobCenter.Models.Enums.EnumDefine;
using QuartzJobCenter.Web.Job;
using QuartzJobCenter.Service.Abstracts;

namespace QuartzJobCenter.Web.SchedulerManager
{
    public class SchedulerCenter : ISchedulerCenter
    {
        private readonly ConcurrentDictionary<string, IScheduler> _schedulerDic = new ConcurrentDictionary<string, IScheduler>();
        private readonly IDbProvider _dbProvider;
        private readonly string _driverDelegateType;
        private readonly ITaskService _taskService;

        public SchedulerCenter(IOptions<QuartzOption> option, ITaskService taskService)
        {
            string dbProviderName = option.Value.DBProviderName;
            string connectionString = option.Value.ConnectionString;
            _driverDelegateType = dbProviderName switch
            {
                "MySql" => typeof(MySQLDelegate).AssemblyQualifiedName,
                "SqlServer" => typeof(SqlServerDelegate).AssemblyQualifiedName,
                "Npgsql" => typeof(PostgreSQLDelegate).AssemblyQualifiedName,
                _ => throw new Exception("dbProviderName unreasonable"),
            };
            _dbProvider = new DbProvider(dbProviderName, connectionString);
            _taskService = taskService;
        }

        private IScheduler GetScheduler(string schedulerName = "httpScheduler")
        {
            if (_schedulerDic != null && _schedulerDic.Count() > 0 && _schedulerDic[schedulerName] != null)
            {
                return _schedulerDic[schedulerName];
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
            DirectSchedulerFactory.Instance.CreateScheduler(schedulerName, "AUTO", new DefaultThreadPool(), jobStore);
            var scheduler = SchedulerRepository.Instance.Lookup(schedulerName).Result;
            scheduler.Start();//默认开始调度器
            _schedulerDic.TryAdd(schedulerName, scheduler);
            return scheduler;
        }

        /// <summary>
        /// 获取job日志
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="schedulerName"></param>
        /// <returns></returns>
        public async Task<List<string>> GetJobLogsAsync(JobKey jobKey, string schedulerName)
        {
            var jobDetail = await GetScheduler(schedulerName).GetJobDetail(jobKey);
            return jobDetail.JobDataMap[ConstantDefine.LOGLIST] as List<string>;
        }

        /// <summary>
        /// 添加调度任务
        /// </summary>
        /// <param name="scheduleEntity"></param>
        /// <returns></returns>
        public async Task<BaseResultResponse> AddScheduleJobAsync(ScheduleEntity entity)
        {
            var result = new BaseResultResponse() { Msg = "添加任务成功！" };
            try
            {
                //检查任务是否已存在
                var jobKey = new JobKey(entity.JobName, entity.JobGroup);
                if (await GetScheduler(entity.SchedulerName).CheckExists(jobKey))
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
                    await GetScheduler(entity.SchedulerName).ScheduleJob(job, trigger);
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
        public async Task<(List<JobInfoEntity>, int totalCount)> GetAllJobAsync(GetAllJobsRequest request)
        {
            List<JobInfoEntity> jobInfoList = new List<JobInfoEntity>();
            var tatolCount = 0;
            var scheduler = GetScheduler(request.SchedulerName);
            if (scheduler != null)
            {
                var qrtzJobDetails = await _taskService.QueryQrtzJobDetailsEntitiesAsync(request);
                //var groupNames = await scheduler.GetJobGroupNames();
                //if (!string.IsNullOrWhiteSpace(request.JobGroup))
                //{
                //    groupNames = groupNames.Where(o => o.Contains(request.JobGroup)).ToList();
                //}
                //foreach (var groupName in groupNames.OrderBy(t => t))
                //{
                //    jobKeyList.AddRange(await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)));
                //}
                //if (!string.IsNullOrWhiteSpace(request.JobName))
                //{
                //    jobKeyList = jobKeyList.Where(o => o.Name.Contains(request.JobName)).ToList();
                //}
                //tatolCount = jobKeyList.Count();
                //jobKeyList = jobKeyList.Skip((request.Page - 1) * request.limit).Take(request.limit).ToList();
                List<JobKey> jobKeyList = new List<JobKey>();
                tatolCount = qrtzJobDetails.Count;
                foreach (var detail in qrtzJobDetails.Data)
                {
                    jobKeyList.Add(new JobKey(detail.JOB_NAME, detail.JOB_GROUP));
                }

                foreach (var jobKey in jobKeyList)
                {
                    var jobDetail = await scheduler.GetJobDetail(jobKey);
                    var triggersList = await scheduler.GetTriggersOfJob(jobKey);
                    var triggers = triggersList.AsEnumerable().FirstOrDefault();

                    var interval = string.Empty;
                    if (triggers is SimpleTriggerImpl)
                        interval = (triggers as SimpleTriggerImpl)?.RepeatInterval.ToString();
                    else
                        interval = (triggers as CronTriggerImpl)?.CronExpressionString;

                    jobInfoList.Add(new JobInfoEntity
                    {
                        GroupName = jobKey.Group,
                        Name = jobKey.Name,
                        LastErrMsg = jobDetail.JobDataMap.GetString(ConstantDefine.EXCEPTION),
                        RequestUrl = jobDetail.JobDataMap.GetString(ConstantDefine.REQUESTURL),
                        TriggerState = await scheduler.GetTriggerState(triggers.Key),
                        PreviousFireTime = triggers.GetPreviousFireTimeUtc()?.LocalDateTime,
                        NextFireTime = triggers.GetNextFireTimeUtc()?.LocalDateTime,
                        BeginTime = triggers.StartTimeUtc.LocalDateTime,
                        Interval = interval,
                        EndTime = triggers.EndTimeUtc?.LocalDateTime,
                        Description = jobDetail.Description
                    });
                }
            }
            return (jobInfoList, tatolCount);
        }

        /// <summary>
        /// 查询任务
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public async Task<ScheduleEntity> QueryJobAsync(string jobGroup, string jobName, string schedulerName)
        {
            var jobKey = new JobKey(jobName, jobGroup);
            var scheduler = GetScheduler(schedulerName);
            var jobDetail = await scheduler.GetJobDetail(jobKey);
            var triggersList = await scheduler.GetTriggersOfJob(jobKey);
            var triggers = triggersList.AsEnumerable().FirstOrDefault();
            var intervalSeconds = (triggers as SimpleTriggerImpl)?.RepeatInterval.TotalSeconds;
            var entity = new ScheduleEntity
            {
                RequestUrl = jobDetail.JobDataMap.GetString(ConstantDefine.REQUESTURL),
                BeginTime = triggers.StartTimeUtc.LocalDateTime,
                EndTime = triggers.EndTimeUtc?.LocalDateTime,
                IntervalSecond = intervalSeconds.HasValue ? Convert.ToInt32(intervalSeconds.Value) : 0,
                JobGroup = jobGroup,
                JobName = jobName,
                Cron = (triggers as CronTriggerImpl)?.CronExpressionString,
                RunTimes = (triggers as SimpleTriggerImpl)?.RepeatCount,
                TriggerType = triggers is SimpleTriggerImpl ? TriggerTypeEnum.Simple : TriggerTypeEnum.Cron,
                RequestType = (RequestTypeEnum)int.Parse(jobDetail.JobDataMap.GetString(ConstantDefine.REQUESTTYPE)),
                RequestParameters = jobDetail.JobDataMap.GetString(ConstantDefine.REQUESTPARAMETERS),
                Headers = jobDetail.JobDataMap.GetString(ConstantDefine.HEADERS),
                MailMessage = (MailMessageEnum)int.Parse(jobDetail.JobDataMap.GetString(ConstantDefine.MAILMESSAGE) ?? "0"),
                Description = jobDetail.Description
            };
            return entity;
        }

        /// <summary>
        /// 暂停/删除 指定的计划
        /// </summary>
        /// <param name="jobGroup">任务分组</param>
        /// <param name="jobName">任务名称</param>
        /// <param name="isDelete">停止并删除任务</param>
        /// <returns></returns>
        public async Task<BaseResultResponse> StopOrDelScheduleJobAsync(string jobGroup, string jobName, string schedulerName, bool isDelete = false)
        {
            BaseResultResponse response;
            var scheduler = GetScheduler(schedulerName);
            try
            {
                await scheduler.PauseJob(new JobKey(jobName, jobGroup));
                if (isDelete)
                {
                    await scheduler.DeleteJob(new JobKey(jobName, jobGroup));
                    response = new BaseResultResponse
                    {
                        Msg = "删除任务计划成功！"
                    };
                }
                else
                {
                    response = new BaseResultResponse
                    {
                        Msg = "停止任务计划成功！"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new BaseResultResponse
                {
                    Code = (int)ResponseCodeEnum.Error,
                    Msg = "停止任务计划失败" + ex.Message
                };
            }
            return response;
        }

        /// <summary>
        /// 恢复运行暂停的任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="jobGroup">任务分组</param>
        /// <param name="schedulerName"></param>
        /// <returns></returns>
        public async Task<BaseResultResponse> ResumeJobAsync(string jobGroup, string jobName, string schedulerName)
        {
            var response = new BaseResultResponse();
            var scheduler = GetScheduler(schedulerName);
            try
            {
                //检查任务是否存在
                var jobKey = new JobKey(jobName, jobGroup);
                if (await scheduler.CheckExists(jobKey))
                {
                    await scheduler.ResumeJob(jobKey);
                    response.Msg = "恢复任务计划成功！";
                }
                else
                {
                    response.Code = (int)ResponseCodeEnum.Fail;
                    response.Msg = "任务不存在";
                }
            }
            catch (Exception ex)
            {
                response.Msg = $"恢复任务计划失败！ex:{ex}";
                response.Code = (int)ResponseCodeEnum.Error;
            }
            return response;
        }

        /// <summary>
        /// 创建类型Cron的触发器
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static ITrigger CreateTrigger(ScheduleEntity entity)
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
                trigger.WithCronSchedule(entity.Cron, cronScheduleBuilder => cronScheduleBuilder.WithMisfireHandlingInstructionDoNothing());//指定cron表达式
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
            var build = trigger.ForJob(entity.JobName, entity.JobGroup)//作业名称
                .Build();
            return build;
        }
    }
}
