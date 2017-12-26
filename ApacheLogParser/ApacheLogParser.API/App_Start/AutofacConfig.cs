using System.Reflection;
using System.Web.Http;
using ApacheLogParser.DAL.Repositories;
using Autofac;
using Autofac.Integration.WebApi;

namespace ApacheLogParser.API
{
    public class AutofacConfig
    {
        public static void Configure()
        {
            var builder = new ContainerBuilder();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<ApacheLogRepository>().As<IApacheLogRepository>();

          

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}