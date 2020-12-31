using QuartzJobCenter.Models.Entities;
using QuartzJobCenter.Models.Request;
using QuartzJobCenter.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuartzJobCenter.Service.Abstracts
{
    public interface ITaskService
    {
        #region select

        Task<ExtendResultResponse<List<QrtzJobDetailsEntity>>> QueryQrtzJobDetailsEntitiesAsync(GetAllJobsRequest request);

        #endregion
    }
}
