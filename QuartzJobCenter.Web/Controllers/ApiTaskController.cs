using Microsoft.AspNetCore.Mvc;
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
    }
}
