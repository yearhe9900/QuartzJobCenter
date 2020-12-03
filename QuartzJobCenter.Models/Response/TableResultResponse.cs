namespace QuartzJobCenter.Models.Response
{
    public class TableResultResponse<T> : BaseResultResponse
    {
        public int Count { get; set; }

        public T Data { get; set; }
    }
}
