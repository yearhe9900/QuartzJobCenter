using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Quartz;
using QuartzJobCenter.Models.Entities;
using QuartzJobCenter.Models.Model;
using QuartzJobCenter.Models.Options;
using QuartzJobCenter.Models.Request;
using QuartzJobCenter.Models.Response;
using QuartzJobCenter.Web.SchedulerManager;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static QuartzJobCenter.Models.Enums.EnumDefine;

namespace QuartzJobCenter.Web.Controllers
{
    public class JobTaskController : Controller
    {
        private readonly ISchedulerCenter _schedulerCenter;
        public readonly List<SchedulerOption> _schedulerOptions;


        public JobTaskController(ISchedulerCenter schedulerCenter, IOptions<List<SchedulerOption>> schedulerOptions)
        {
            _schedulerCenter = schedulerCenter;
            _schedulerOptions = schedulerOptions.Value;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GrpJobIndex()
        {
            return View();
        }

        public async Task<IActionResult> AddOrEditJobViewAsync(string name, string groupName, int schedulerType)
        {
            var schedulerName = _schedulerOptions.Where(o => o.ScheduleTypeId == schedulerType).FirstOrDefault().SchedulerName;
            ViewBag.SchedulerName = schedulerName;
            if (!string.IsNullOrWhiteSpace(name) & !string.IsNullOrWhiteSpace(groupName))
            {
                var queryJobInfo = await _schedulerCenter.QueryJobAsync(groupName, name, schedulerName);
                return View(queryJobInfo);
            }
            return View(null);
        }

        public async Task<IActionResult> AddOrEditGrpcJobView(string name, string groupName, int schedulerType)
        {
            var schedulerName = _schedulerOptions.Where(o => o.ScheduleTypeId == schedulerType).FirstOrDefault().SchedulerName;
            ViewBag.SchedulerName = schedulerName;
            if (!string.IsNullOrWhiteSpace(name) & !string.IsNullOrWhiteSpace(groupName))
            {
                var queryJobInfo = await _schedulerCenter.QueryJobAsync(groupName, name, schedulerName);
                return View(queryJobInfo);
            }
            return View(null);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllJobs(GetAllJobsRequest request)
        {
            var (allJobs, totalCount) = await _schedulerCenter.GetAllJobAsync(request);
            var response = new ExtendResultResponse<List<JobInfoEntity>>()
            {
                Count = totalCount,
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
                    var stopResult = await _schedulerCenter.StopOrDelScheduleJobAsync(entity.OldGroupName, entity.OldName, entity.SchedulerName, true);
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
        public async Task<IActionResult> GetJobLogs(string name, string groupName, int schedulerType)
        {
            var schedulerName = _schedulerOptions.Where(o => o.ScheduleTypeId == schedulerType).FirstOrDefault().SchedulerName;
            var jobLogs = await _schedulerCenter.GetJobLogsAsync(new JobKey(name, groupName), schedulerName);
            List<LogInfoModel> logInfoModels = new List<LogInfoModel>();
            if (jobLogs != null)
            {
                foreach (var log in jobLogs)
                {
                    logInfoModels.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<LogInfoModel>(log));
                }
            }
            return View(logInfoModels);
        }

        /// <summary>
        /// 暂停任务、删除任务、恢复运行暂停的任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DoOperationJob(BaseOperationRequest request)
        {
            var schedulerName = _schedulerOptions.Where(o => o.ScheduleTypeId == (int)request.SchedulerType).FirstOrDefault().SchedulerName;
            var response = request.OperationType switch
            {
                OperationTypeEnum.StopJob => await _schedulerCenter.StopOrDelScheduleJobAsync(request.GroupName, request.Name, schedulerName),
                OperationTypeEnum.RemoveJob => await _schedulerCenter.StopOrDelScheduleJobAsync(request.GroupName, request.Name, schedulerName, true),
                OperationTypeEnum.ResumeJob => await _schedulerCenter.ResumeJobAsync(request.GroupName, request.Name, schedulerName),
                _ => new BaseResultResponse() { Code = (int)ResponseCodeEnum.Fail, Msg = "修改计划失败" },
            };
            return new JsonResult(response);
        }
    }
}
