using Microsoft.Extensions.DependencyInjection;
using QuartzJobCenter.Web.SchedulerManager;
using System.Linq;
using System.Reflection;

namespace QuartzJobCenter.Web.ConfigureServicesExtensions
{
    /// <summary>
    /// 服务注册扩展类
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 单例注册
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSingletonSetting(this IServiceCollection services)
        {
            #region service & repository

            var dal = Assembly.Load("QuartzJobCenter.Repository");
            services.AddSingletonExtension(dal);
            var bll = Assembly.Load("QuartzJobCenter.Service");
            services.AddSingletonExtension(bll);

            #endregion

            //#region rabbitMq
            //services.AddSingleton<IRabbitMQClient, RabbitMQClient>();
            //services.AddSingleton<IMQProducerClient, MQProducerClient>();
            ////services.AddSingleton<IMQConsumerClient, MQConsumerClient>();
            //#endregion

            //#region redis
            //services.AddSingleton<IRedisCacheClient, RedisCacheClient>();
            //#endregion

            //#region RestHttpClient

            //services.AddSingleton<IRestHttpClient, RestHttpClient>();

            //#endregion

            #region Validators

            //services.AddSingleton<IValidator<CommandStateRequest>, CommandStateValidator>();

            #endregion

            #region Quartz SchedulerCenter

            services.AddSingleton<ISchedulerCenter, SchedulerCenter>();

            #endregion

            return services;
        }

        /// <summary>
        /// 批量Singleton模式注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly">类库</param>
        private static void AddSingletonExtension(this IServiceCollection services, Assembly assembly)
        {
            var interfaces = assembly.GetTypes().Where(t => t.IsInterface);
            var implements = assembly.GetTypes().Where(t => t.IsClass);
            foreach (var item in interfaces)
            {
                var type = implements.FirstOrDefault(x => item.IsAssignableFrom(x));
                if (type != null)
                {
                    services.AddSingleton(item, type);
                }
            }
        }

        /// <summary>
        /// 批量Scoped模式注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly">类库</param>
        private static void AddScopedExtension(this IServiceCollection services, Assembly assembly)
        {
            var interfaces = assembly.GetTypes().Where(t => t.IsInterface);
            var implements = assembly.GetTypes().Where(t => t.IsClass);
            foreach (var item in interfaces)
            {
                var type = implements.FirstOrDefault(x => item.IsAssignableFrom(x));
                if (type != null)
                {
                    services.AddScoped(item, type);
                }
            }
        }

        /// <summary>
        /// 批量Transient模式注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly">类库</param>
        private static void AddTransientExtension(this IServiceCollection services, Assembly assembly)
        {
            var interfaces = assembly.GetTypes().Where(t => t.IsInterface);
            var implements = assembly.GetTypes().Where(t => t.IsClass);
            foreach (var item in interfaces)
            {
                var type = implements.FirstOrDefault(x => item.IsAssignableFrom(x));
                if (type != null)
                {
                    services.AddTransient(item, type);
                }
            }
        }
    }
}
