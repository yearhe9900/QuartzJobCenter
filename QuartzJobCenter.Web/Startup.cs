using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuartzJobCenter.Common.DapperManager;
using QuartzJobCenter.Models.Define;
using QuartzJobCenter.Models.Enums;
using QuartzJobCenter.Models.Options;
using QuartzJobCenter.Web.ConfigureServicesExtensions;
using System;
using System.Collections.Generic;

namespace QuartzJobCenter.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<List<SchedulerOption>>(Configuration.GetSection("SchedulerOptions"));

            #region ×¢²áDapper
            var connets = Configuration.GetSection(ConstantDefine.DbConnedtions).GetChildren();
            var dbDic = new Dictionary<string, DapperClient>();
            foreach (var item in connets)
            {
                string name = item.GetSection("Name").Value;
                string type = item.GetSection("DBType").Value;
                string constr = item.GetSection("ConnectionString").Value;
                ConnectionConfig conf = new ConnectionConfig
                {
                    ConnectionString = constr
                };
                switch (type)
                {
                    case ConstantDefine.SqlServer:
                        conf.DbType = EnumDbStoreType.SqlServer;
                        break;
                }
                DapperClient client = new DapperClient(conf);
                dbDic.Add(name, client);
            }
            services.AddClient(dbDic);
            #endregion

            services.AddControllersWithViews();
            services.AddSingletonSetting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    
    }
}
