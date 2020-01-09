using Autofac;
using HttpReports.Web.NetFrame.App_Start;
using HttpReports.Web.Services;
using Quartz;
using System;
using System.Threading.Tasks;

namespace HttpReports.Web.Job
{
    public class BackendJob : IJob
    {
        public DataService dataService;

        public Task Execute(IJobExecutionContext context)
        {
            dataService = AutofacConfig.Container.Resolve<DataService>();

            var job = context.JobDetail.JobDataMap.Get("job") as Models.Job;

            Console.WriteLine($"--- {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------------- {job.Title} -------------");
            Console.WriteLine();
            Console.WriteLine();

            dataService.CheckJob(job);

            return Task.FromResult(true);
        }

    }
}
