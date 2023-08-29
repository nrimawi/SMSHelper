using log4net;
using log4net.Config;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SMSHelper.Configurations;
using SMSHelper.Helper.Implementation;

namespace SMSHelper
{
    public class program
    {
        public static void Main(String[] args)
        {
            #region Configure Logging
            var log4netRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(log4netRepository, new FileInfo("Logging.config"));
            #endregion

            var host = CreateHostDefaultBuilder(args).Build();
            host.RunAsync();
            RunTask(host.Services);
        }


        public static IHostBuilder CreateHostDefaultBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder()
                       .ConfigureAppConfiguration(app => { app.AddJsonFile("AppSettings.json"); })
                       .ConfigureServices((_, services) =>
                       {
                           services.ConfigureDAL(_.Configuration)
                                   .ConfigureBusiness();
                       });
        }

        public static void RunTask(IServiceProvider services)
        {

            var taskHelper = services.GetService(typeof(TaskHelper)) as TaskHelper;
            Task.Run(() => taskHelper?.Execute(true)).Wait();
        }
    }
}