using NLog;
using Quartz;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DakaPathAppFileService.Jobs
{
    public class TempCleanJob : IJob
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists(App.SystemTempPath))
                {
                    return;
                }
                DirectoryInfo directory = new DirectoryInfo(App.SystemTempPath);
                if (directory == null)
                {
                    return;
                }
                DateTime now = DateTime.Now.AddMinutes(-5);
                var fileList = directory.GetFiles().Where(x => x.CreationTime <= now).ToList();
                fileList.ForEach(x =>
                {
                    try
                    {
                        if (x.CreationTime <= now)
                        {
                            x.Delete();
                        }
                    }
                    catch
                    {
                        return;
                    }
                });
                return;
            });
        }
    }
}
