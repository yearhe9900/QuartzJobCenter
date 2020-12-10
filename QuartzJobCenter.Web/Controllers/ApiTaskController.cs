using Microsoft.AspNetCore.Mvc;
using Quartz;
using QuartzJobCenter.Models.Entities;
using QuartzJobCenter.Models.Response;
using QuartzJobCenter.Web.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzJobCenter.Web.Controllers
{
    public class ApiTaskController : Controller
    {
        private SchedulerCenter _schedulerCenter;

        public ApiTaskController(SchedulerCenter schedulerCenter)
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
        public async Task<IActionResult> GetAllApiTaskJobs()
        {
            var allJobs = await _schedulerCenter.GetAllJobAsync();
            var response = new TableResultResponse<List<JobInfoEntity>>()
            {
                Count = 0,
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
        public async Task<List<string>> GetJobLogs(string name, string group)
        {
            var jobKey = new JobKey(name, group);
            return await _schedulerCenter.GetJobLogsAsync(jobKey);
        }

    }
}
