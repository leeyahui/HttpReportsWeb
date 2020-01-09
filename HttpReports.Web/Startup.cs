using HttpReports.Web.DataAccessors;
using HttpReports.Web.DataContext;
using HttpReports.Web.Filters;
using HttpReports.Web.Implements;
using HttpReports.Web.Job;
using HttpReports.Web.Models;
using HttpReports.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HttpReports.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            DependencyInjection(services);

            services.AddMvc(x =>
            {
                // ȫ�ֹ�����
                x.Filters.Add<GlobalAuthorizeFilter>();
                x.Filters.Add<GlobalExceptionFilter>();

            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            app.UseCookiePolicy();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void DependencyInjection(IServiceCollection services)
        {
            services.AddSingleton<HttpReportsConfig>();
            services.AddSingleton<JobService>();
            services.AddSingleton<ScheduleService>();

            services.AddTransient<DBFactory>();

            services.AddTransient<DataService>();

            // ע�����ݿ������
            RegisterDBService(services);


            // ��ʼ��ϵͳ����
            InitWebService(services);
        }

        private void InitWebService(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();

            ServiceContainer.provider = provider;

            // ��ʼ�����ݿ��
            provider.GetService<DBFactory>().InitDB();

            // ������̨����
            provider.GetService<JobService>().Start();

        }

        private void RegisterDBService(IServiceCollection services)
        {
            string dbType = Configuration["HttpReportsConfig:DBType"];

            if (dbType.ToLower() == "sqlserver")
            {
                services.AddTransient<IDataAccessor, DataAccessorSqlServer>();
            }
            else if (dbType.ToLower() == "mysql")
            {
                services.AddTransient<IDataAccessor, DataAccessorMySql>();
            }
            else if (dbType.ToLower() == "oracle")
            {
                services.AddTransient<IDataAccessor, DataAccessorOracle>();
            }
            else
            {
                throw new Exception("���ݿ����ô���");
            }
        }
    }
}
