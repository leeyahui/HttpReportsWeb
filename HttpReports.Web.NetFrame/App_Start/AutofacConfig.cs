using Autofac;
using Autofac.Integration.Mvc;
using HttpReports.Web.DataAccessors;
using HttpReports.Web.DataContext;
using HttpReports.Web.Job;
using HttpReports.Web.Models;
using HttpReports.Web.Services;
using System;
using System.Web.Mvc;

namespace HttpReports.Web.NetFrame.App_Start
{
    public class AutofacConfig
    {
        public static IContainer Container;
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialise()
        {
            var builder = RegisterService();
            Container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));
            // 初始化数据库表
            Container.Resolve<DBFactory>().InitDB();

            // 开启后台任务
            Container.Resolve<JobService>().Start();

        }

        /// <summary>
        /// 注入实现
        /// </summary>
        /// <returns></returns>
        private static ContainerBuilder RegisterService()
        {
            var builder = new ContainerBuilder();
            // Register your MVC controllers. (MvcApplication is the name of
            // the class in Global.asax.)
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            builder.RegisterType<HttpReportsConfig>().SingleInstance();
            builder.RegisterType<JobService>();
            builder.RegisterType<ScheduleService>().SingleInstance();
            builder.RegisterType<DBFactory>().SingleInstance();
            builder.RegisterType<DataService>();

            var dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

            if (dbType.ToLower() == "sqlserver")
            {
                builder.RegisterType<DataAccessorSqlServer>().As<IDataAccessor>().SingleInstance();
            }
            else if (dbType.ToLower() == "mysql")
            {
                builder.RegisterType<DataAccessorMySql>().As<IDataAccessor>().SingleInstance();
            }
            else if (dbType.ToLower() == "oracle")
            {
                builder.RegisterType<DataAccessorOracle>().As<IDataAccessor>().SingleInstance();
            }
            else
            {
                throw new Exception("数据库配置错误！");
            }
            return builder;
        }
    }
}