using Microsoft.AspNetCore.Mvc;
using Quartz;
using QuartzJobCenter.Models.Entities;
using QuartzJobCenter.Models.Request;
using QuartzJobCenter.Models.Response;
using QuartzJobCenter.Web.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzJobCenter.Web.Controllers
{
    public class JobTaskController : Controller
    {
        private SchedulerCenter _schedulerCenter;

        public JobTaskController(SchedulerCenter schedulerCenter)
        {
            _schedulerCenter = schedulerCenter;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LogView()
        {
            return View();
        }

        public IActionResult AddJobView()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllJobs()
        {
            var allJobs = await _schedulerCenter.GetAllJobAsync();
            var response = new ExtendResultResponse<List<JobInfoEntity>>()
            {
                Count = allJobs.Count,
                Data = allJobs
            };
            return new JsonResult(response);
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddJob(ScheduleEntity entity)
        {
            var response = await _schedulerCenter.AddScheduleJobAsync(entity);
            return new JsonResult(response);
        }

        /// <summary>
        /// 获取job日志
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetJobLogs(string name, string groupName)
        {
            var jobLogs = await _schedulerCenter.GetJobLogsAsync(new JobKey(name, groupName));
            var response = new ExtendResultResponse<List<string>>()
            {
                Count = jobLogs.Count,
                Data = jobLogs
            };
            return new JsonResult(response);
        }

        /// <summary>
        /// 暂停任务、删除任务、恢复运行暂停的任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DoOperationJob(BaseOperationRequest request)
        {
            BaseResultResponse response = request.OperationType switch
            {
                Common.Define.EnumDefine.OperationTypeEnum.StopJob => await _schedulerCenter.StopOrDelScheduleJobAsync(request.GroupName, request.Name),
                Common.Define.EnumDefine.OperationTypeEnum.RemoveJob => await _schedulerCenter.StopOrDelScheduleJobAsync(request.GroupName, request.Name, true),
                Common.Define.EnumDefine.OperationTypeEnum.ResumeJob => await _schedulerCenter.ResumeJobAsync(request.GroupName, request.Name),
                _ => new BaseResultResponse(),
            };
            return new JsonResult(response);
        }
    }
}
