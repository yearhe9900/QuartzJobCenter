namespace QuartzJobCenter.Common.DapperManager
{
    public interface IDapperFactory
    {
        DapperClient CreateClient(string name);
    }
}
