using DakaPathAppFileService.Jobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;

namespace DakaPathAppFileService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            App.ReadSystemConfig();

            IScheduler scheduler;
            ISchedulerFactory factory;
            factory = new StdSchedulerFactory();
            scheduler = factory.GetScheduler().Result;
            scheduler.Start().Wait();
            IJobDetail job = JobBuilder.Create<TempCleanJob>().WithIdentity("TempCleanJob", "Group").Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("TempCleanJob", "Group")
                .WithCronSchedule(App.SystemTempCleanJobCron)
                .Build();
            scheduler.ScheduleJob(job, trigger).Wait();
            scheduler.Start().Wait();

            CreateHostBuilder(args).Build().Run();

            scheduler.Shutdown();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
