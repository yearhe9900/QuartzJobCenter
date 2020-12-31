using Dapper;
using QuartzJobCenter.Common.DapperManager;
using QuartzJobCenter.Models.Entities;
using QuartzJobCenter.Models.Request;
using QuartzJobCenter.Models.Response;
using QuartzJobCenter.Repository.Abstracts;
using QuartzJobCenter.Service.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzJobCenter.Service.Impl
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;


        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        #region select

        public async Task<ExtendResultResponse<List<QrtzJobDetailsEntity>>> QueryQrtzJobDetailsEntitiesAsync(GetAllJobsRequest request)
        {
            return await _taskRepository.QueryQrtzJobDetailsEntitiesAsync(request);
        }

        #endregion
    }
}
