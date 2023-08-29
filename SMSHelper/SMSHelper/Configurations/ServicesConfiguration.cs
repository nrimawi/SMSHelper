
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SMSHelper.Common;
using SMSHelper.Helper.Implementation;
using SMSHelper.Helper;

namespace SMSHelper.Configurations
{
    public static class ServicesConfiguration
    {

        public static IServiceCollection ConfigureDAL(this IServiceCollection services, IConfiguration configuration)
        {

            var mongoDbSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            services.AddSingleton<IMongoClient>(s => new MongoClient(mongoDbSettings.ConnectionString));
            services.AddScoped<IHTDHelper, HTDHelper>();

            return services;
        }

        public static IServiceCollection ConfigureBusiness(this IServiceCollection services)
        {
            services.AddTransient<TaskHelper>();
            return services;
        }
    }

}
