using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using RestApp.DataDomain.Configuration;
using RestApp.Utilities;
using System.Reflection;

namespace RestApp
{
    public class Startup
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfigurationOptions>(_ =>
            {
                var config = new ConfigurationOptions();
                Configuration.Bind(config);
                return config;
            });
            services.AddSingleton<Utilities.ILogger, CustomLogger>();
            services.AddTransient<IRestClient, RetryableRestClient>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
          
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
