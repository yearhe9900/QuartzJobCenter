using System;
using Microsoft.Extensions.DependencyInjection;

namespace QuartzJobCenter.Common.DapperManager
{
    public interface IDapperFactoryBuilder
    {
        string Name { get; }

        IServiceCollection Services { get; }
    }
}
