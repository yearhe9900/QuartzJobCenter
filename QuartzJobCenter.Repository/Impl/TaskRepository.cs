using Dapper;
using QuartzJobCenter.Common.DapperManager;
using QuartzJobCenter.Models.Entities;
using QuartzJobCenter.Models.Request;
using QuartzJobCenter.Models.Response;
using QuartzJobCenter.Repository.Abstracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzJobCenter.Repository.Impl
{
    public class TaskRepository : ITaskRepository
    {
        private DapperClient _masterClient;

        public TaskRepository(IDapperClientFactory dapperFactory)
        {
            _masterClient = dapperFactory.GetTaskCenterConnection();
        }

        #region select

        public async Task<ExtendResultResponse<List<QrtzJobDetailsEntity>>> QueryQrtzJobDetailsEntitiesAsync(GetAllJobsRequest request)
        {
            var list = new ExtendResultResponse<List<QrtzJobDetailsEntity>>() { };
            string sql = @"SELECT [SCHED_NAME]
      ,[JOB_NAME]
      ,[JOB_GROUP]
      ,[DESCRIPTION]
      ,[JOB_CLASS_NAME]
      ,[IS_DURABLE]
      ,[IS_NONCONCURRENT]
      ,[IS_UPDATE_DATA]
      ,[REQUESTS_RECOVERY]
      ,[JOB_DATA]
  FROM [TASKCENTER].[dbo].[QRTZ_JOB_DETAILS](Nolock)
                  WHERE [SCHED_NAME]=@SCHED_NAME {0}
                ORDER BY [JOB_GROUP],[JOB_NAME] desc   OFFSET @skip ROW  FETCH NEXT @take ROW ONLY;
                SELECT COUNT(*) AS RecordTotal FROM [TASKCENTER].[dbo].[QRTZ_JOB_DETAILS] WHERE [SCHED_NAME]=@SCHED_NAME {0} ";

            DynamicParameters dbparas = new DynamicParameters();
            dbparas.Add("@skip", request.limit * (request.Page - 1));
            dbparas.Add("@take", request.limit);
            dbparas.Add("@SCHED_NAME", request.SchedulerName);

            string filterSql = string.Empty;

            if (!string.IsNullOrWhiteSpace(request.JobGroup))
            {
                filterSql += " AND JOB_GROUP like @JOB_GROUP";
                dbparas.Add("@JOB_GROUP", $"%{request.JobGroup}%");
            }
            if (!string.IsNullOrWhiteSpace(request.JobName))
            {
                filterSql += " AND [JOB_NAME] like @JOB_NAME";
                dbparas.Add("@JOB_NAME", $"%{request.JobName}%");
            }
            sql = string.Format(sql, filterSql);

            var (entityList, count) = await _masterClient.QueryMultipleAsync<QrtzJobDetailsEntity>(sql, dbparas);
            list.Data = entityList;
            list.Count = count;
            return list;
        }

        #endregion
    }
}
