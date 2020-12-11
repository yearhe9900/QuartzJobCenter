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
using static QuartzJobCenter.Common.Define.EnumDefine;

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

        public async Task<IActionResult> AddOrEditJobViewAsync(string name, string groupName)
        {
            if (!string.IsNullOrWhiteSpace(name) & !string.IsNullOrWhiteSpace(groupName))
            {
                var queryJobInfo = await _schedulerCenter.QueryJobAsync(groupName, name);
                return View(queryJobInfo);
            }
            return View(null);
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
        public async Task<IActionResult> AddOrEditJob(ScheduleEntity entity)
        {
            BaseResultResponse response;
            switch (entity.IsEdit)
            {
                case "0": response = await _schedulerCenter.AddScheduleJobAsync(entity); break;
                case "1":
                    var stopResult = await _schedulerCenter.StopOrDelScheduleJobAsync(entity.OldGroupName, entity.OldName, true);
                    if (stopResult.Code == (int)ResponseCodeEnum.Success)
                    {
                        var addResult = await _schedulerCenter.AddScheduleJobAsync(entity);
                        if (addResult.Code == (int)ResponseCodeEnum.Success)
                        {
                            response = new BaseResultResponse() { Msg = "重新添加计划成功" };
                        }
                        else
                        {
                            response = addResult;
                        }
                    }
                    else
                    {
                        response = stopResult;
                    }
                    ; break;
                default: response = new BaseResultResponse() { Code = (int)ResponseCodeEnum.Fail, Msg = "编辑计划失败" }; break;
            }
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
            var response = request.OperationType switch
            {
                OperationTypeEnum.StopJob => await _schedulerCenter.StopOrDelScheduleJobAsync(request.GroupName, request.Name),
                OperationTypeEnum.RemoveJob => await _schedulerCenter.StopOrDelScheduleJobAsync(request.GroupName, request.Name, true),
                OperationTypeEnum.ResumeJob => await _schedulerCenter.ResumeJobAsync(request.GroupName, request.Name),
                _ => new BaseResultResponse() { Code = (int)ResponseCodeEnum.Fail, Msg = "修改计划失败" },
            };
            return new JsonResult(response);
        }
    }
}
