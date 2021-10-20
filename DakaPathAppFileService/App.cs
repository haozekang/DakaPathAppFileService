using Chloe;
using Chloe.SQLite;
using DakaPathAppFileService.ConfigModels;
using DakaPathAppFileService.DataHelper;
using DakaPathAppFileService.ExtendMethod;
using NLog;
using SharpYaml.Serialization;
using System;
using System.IO;

namespace DakaPathAppFileService
{
    public class App
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static string SQLiteConnectionString;

        public static string MinIOEndPoint;
        public static string MinIOAPIEndPoint;
        public static bool? MinIOUseSsl;
        public static string MinIOFileBucket;
        public static string MinIOImageBucket;
        public static string MinIOReportBucket;
        public static string MinIOAccessKey;
        public static string MinIOSecertKey;

        public static string SystemTitle;
        public static string SystemCompany;
        public static string SystemTempPath;
        public static string SystemTempCleanJobCron;
        public static int YearTime => DateTime.Now.Year;

        public static DbContext sqlite
        {
            get
            {
                return SQLiteConnectionString.IsBlank() ? null : new SQLiteContext(new SQLiteConnectionFactory(SQLiteConnectionString));
            }
        }

        public static void ReadSystemConfig()
        {
            try
            {
                var serializer = new Serializer();
                string configConfigPath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "config.yml");
                if (configConfigPath.IsBlank())
                {
                    return;
                }
                if (!File.Exists(configConfigPath))
                {
                    return;
                }
                using (Stream f = new FileStream(configConfigPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    DakaPathAppFileServiceYamlConfigModel Config = serializer.Deserialize<DakaPathAppFileServiceYamlConfigModel>(f);
                    SQLiteConnectionString = Config.Databases.SQLite.GetConnectionString();

                    MinIOEndPoint = Config.MinIO.EndPoint;
                    MinIOAPIEndPoint = Config.MinIO.APIEndPoint;
                    MinIOUseSsl = Config.MinIO.UseSsl;
                    MinIOFileBucket = Config.MinIO.FileBucket;
                    MinIOImageBucket = Config.MinIO.ImageBucket;
                    MinIOReportBucket = Config.MinIO.ReportBucket;
                    MinIOAccessKey = Config.MinIO.AccessKey;
                    MinIOSecertKey = Config.MinIO.SecertKey;

                    SystemTitle = Config.System.Title;
                    SystemCompany = Config.System.Company;
                    SystemTempPath = Config.System.TempPath;
                    SystemTempCleanJobCron = Config.System.TempCleanJobCron;
                }
                logger.Trace($"配置载入完毕！");
            }
            catch (Exception e)
            {
                logger.Error($"配置载入失败: {e}");
            }
        }

        public App()
        {
            var logoDirPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            NLog.GlobalDiagnosticsContext.Set("LogPath", logoDirPath);
        }
    }
}
