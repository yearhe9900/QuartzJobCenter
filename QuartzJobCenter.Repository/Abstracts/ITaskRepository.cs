using QuartzJobCenter.Models.Entities;
using QuartzJobCenter.Models.Request;
using QuartzJobCenter.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuartzJobCenter.Repository.Abstracts
{
    public interface ITaskRepository
    {
        #region select

        Task<ExtendResultResponse<List<QrtzJobDetailsEntity>>> QueryQrtzJobDetailsEntitiesAsync(GetAllJobsRequest request);

        #endregion
    }
}
