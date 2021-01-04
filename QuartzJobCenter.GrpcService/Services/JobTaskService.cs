using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzJobCenter.GrpcService
{
    public class JobTaskService : JobTask.JobTaskBase
    {
        private readonly ILogger<JobTaskService> _logger;
        public JobTaskService(ILogger<JobTaskService> logger)
        {
            _logger = logger;
        }

        public override Task<ExcuteReply> Excute(ExcuteRequest request, ServerCallContext context)
        {
            Console.Out.WriteLine(request.Params);
            return Task.FromResult(new ExcuteReply
            {
                Message = "Hello " + request.Params,
                Status = Stauts.Success
            });
        }
    }
}
