using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzJobCenter.Models.Request
{
    public class GetAllJobsRequest : BasePageRequest
    {
        public string JobGroup { get; set; }

        public string JobName { get; set; }
    }
}
