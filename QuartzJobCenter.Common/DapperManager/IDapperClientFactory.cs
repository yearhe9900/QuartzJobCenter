namespace QuartzJobCenter.Common.DapperManager
{
    public interface IDapperClientFactory
    {
        DapperClient GetClient(string name);

        DapperClient GetTaskCenterConnection();
    }
}
