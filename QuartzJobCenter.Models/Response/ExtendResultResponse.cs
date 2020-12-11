namespace QuartzJobCenter.Models.Response
{
    public class ExtendResultResponse<T> : BaseResultResponse
    {
        public int Count { get; set; }

        public T Data { get; set; }
    }
}
