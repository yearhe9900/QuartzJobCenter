using System;

namespace QuartzJobCenter.Models.Dtos
{
    public class ErrorViewDto
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
