using System;
using KX.StatusService.Configuration;
using KX.StatusService.Extensions;
using KX.StatusService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using KX.StatusService.Swagger;
using KX.StatusService.Middlewares;

namespace KX.Gateway
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMemoryCache();
            services.AddHealthChecks();

            //Configure status endpoint dependencies
            services.AddSingleton<IStatusService, KX.StatusService.Services.StatusService>();
            services.AddSingleton((services) => configuration.GetSection("StatusServiceConfig")
                .Get<StatusServiceConfig>());

            if (env.IsDevelopment())
            {
                services.AddCustomSwagger();
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseCustomSwagger(configuration["swagger:path"]);
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapCustomHealthChecks("/healthz", $"StatusService - {Guid.NewGuid()}");
                endpoints.MapControllers();
            });
        }
    }
}
