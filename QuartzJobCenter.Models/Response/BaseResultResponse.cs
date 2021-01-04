using static QuartzJobCenter.Models.Enums.EnumDefine;

namespace QuartzJobCenter.Models.Response
{
    public class BaseResultResponse
    {
        public int Code { get; set; } = (int)ResponseCodeEnum.Success;

        public string Msg { get; set; }
    }
}
