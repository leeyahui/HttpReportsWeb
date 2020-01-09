using Autofac;
using HttpReports.Web.NetFrame.App_Start;
using HttpReports.Web.Services;
using Quartz;
using System.Threading.Tasks;

namespace HttpReports.Web.Job
{
    public class CurrentJob : BaseJob
    {
        public override string cron => "0 0/1 * * * ?";

        public override Task Execute(IJobExecutionContext context)
        {
            var dataService = AutofacConfig.Container.Resolve<DataService>();

            var scheduleService = AutofacConfig.Container.Resolve<ScheduleService>();

            var Jobs = dataService.GetJobs();

            foreach (var k in Jobs)
            {
                var job = scheduleService.scheduler.GetJobDetail(new JobKey("T" + k.Id, "HttpReports")).Result;

                if (job == null)
                {
                    if (k.Status == 1)
                    {
                        ScheduleJob(scheduleService, k);
                    }
                }
                else
                {
                    if (k.Status == 0)
                    {
                        DeleteJob(scheduleService, job);
                    }
                }
            }


            return Task.FromResult(true);
        }

        public void ScheduleJob(ScheduleService scheduleService, Models.Job k)
        {
            var job = JobBuilder.Create<BackendJob>().
                WithIdentity("T" + k.Id, "HttpReports")
                .SetJobData(new JobDataMap { { "job", k } }).Build();

            var trigger = TriggerBuilder.Create().WithCronSchedule(k.CronLike).Build();

            scheduleService.scheduler.ScheduleJob(job, trigger);
        }

        private void DeleteJob(ScheduleService scheduleService, IJobDetail job)
        {
            if (scheduleService.scheduler.CheckExists(job.Key).Result)
            {
                scheduleService.scheduler.PauseJob(job.Key).Wait();

                scheduleService.scheduler.DeleteJob(job.Key).Wait();
            }
        }
    }
}
