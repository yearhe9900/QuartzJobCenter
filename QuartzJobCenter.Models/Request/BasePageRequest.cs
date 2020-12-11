using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzJobCenter.Models.Request
{
    public class BasePageRequest
    {
        public int Page { get; set; }
        public int limit { get; set; }
    }
}
