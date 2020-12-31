using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QuartzJobCenter.Common.Define;

namespace QuartzJobCenter.Common.DapperManager
{
    public static class DapperClientCollectionExtensions
    {
        public static IServiceCollection AddClient(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddOptions();

            services.AddSingleton<DapperClientFactory>();
            services.TryAddSingleton<IDapperClientFactory>(serviceProvider => serviceProvider.GetRequiredService<DapperClientFactory>());

            return services;
        }

        public static IDapperFactoryBuilder AddClient(this IServiceCollection services, Dictionary<string, DapperClient> clientDic)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));


            if (clientDic == null)
                throw new ArgumentNullException(nameof(clientDic));

            AddClient(services);
            var builder = new DefaultDapperFactoryBuilder(services, ConstantDefine.DbConnedtions);

            builder.ConfigureDapperDictionary(clientDic);

            return builder;
        }

        public static IDapperFactoryBuilder ConfigureDapperDictionary(this IDapperFactoryBuilder builder, Dictionary<string, DapperClient> clientDic)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (clientDic == null)
                throw new ArgumentNullException(nameof(clientDic));

            builder.Services.Configure<DapperClientListOptions>(builder.Name, options => options.DapperDictionary= clientDic);

            return builder;
        }

    }
}
