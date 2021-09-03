using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Web.Framework.Infrastructure
{
    public class NopStaticFilesStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddNopWebOptimizer();
        }

        public void Configure(IApplicationBuilder application)
        {
            //WebOptimizer should be placed before configuring static files
            application.UseNopWebOptimizer();

            //use static files feature
            application.UseNopStaticFiles();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 99; //Static files should be registered before routing & custom middlewares
    }
}