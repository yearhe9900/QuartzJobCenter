using Quartz;
using QuartzJobCenter.Models.Entities;
using QuartzJobCenter.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuartzJobCenter.Models.Request;

namespace QuartzJobCenter.Web.SchedulerManager
{
    public interface ISchedulerCenter
    {
        /// <summary>
        /// 获取job日志
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="schedulerName"></param>
        /// <returns></returns>
        Task<List<string>> GetJobLogsAsync(JobKey jobKey, string schedulerName);

        /// <summary>
        /// 添加调度任务
        /// </summary>
        /// <param name="scheduleEntity"></param>
        /// <returns></returns>
        Task<BaseResultResponse> AddScheduleJobAsync(ScheduleEntity entity);

        /// <summary>
        /// 获取所有Job（详情信息 - 初始化页面调用）
        /// </summary>
        /// <returns></returns>
        Task<(List<JobInfoEntity>, int totalCount)> GetAllJobAsync(GetAllJobsRequest request);

        /// <summary>
        /// 查询任务
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        Task<ScheduleEntity> QueryJobAsync(string jobGroup, string jobName, string schedulerName);

        /// <summary>
        /// 暂停/删除 指定的计划
        /// </summary>
        /// <param name="jobGroup">任务分组</param>
        /// <param name="jobName">任务名称</param>
        /// <param name="isDelete">停止并删除任务</param>
        /// <returns></returns>
        Task<BaseResultResponse> StopOrDelScheduleJobAsync(string jobGroup, string jobName, string schedulerName, bool isDelete = false);

        /// <summary>
        /// 恢复运行暂停的任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="jobGroup">任务分组</param>
        /// <param name="schedulerName"></param>
        /// <returns></returns>
        Task<BaseResultResponse> ResumeJobAsync(string jobGroup, string jobName, string schedulerName);
    }
}
