using Microsoft.AspNetCore.Mvc;
using Quartz;
using QuartzJobCenter.Models.Entities;
using QuartzJobCenter.Models.Response;
using QuartzJobCenter.Web.Components;
using System.Collections.Generic;
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

        ///// <summary>
        ///// 暂停任务
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> StopJob([FromBody] JobKey job)
        //{
        //    return await scheduler.StopOrDelScheduleJobAsync(job.Group, job.Name);
        //}

        ///// <summary>
        ///// 删除任务
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> RemoveJob([FromBody] JobKey job)
        //{
        //    return await scheduler.StopOrDelScheduleJobAsync(job.Group, job.Name, true);
        //}

        ///// <summary>
        ///// 恢复运行暂停的任务
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> ResumeJob([FromBody] JobKey job)
        //{
        //    return await scheduler.ResumeJobAsync(job.Group, job.Name);
        //}
    }
}