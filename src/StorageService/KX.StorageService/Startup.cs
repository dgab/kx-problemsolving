using System;
using KX.StorageService.Extensions;
using KX.StorageService.Middlewares;
using KX.StorageService.Services;
using KX.StorageService.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KX.StorageService
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
            services.AddHealthChecks();

            //Configure status endpoint dependencies
            services.AddSingleton<IDataProvider, InMemoryDataProvider>();

            if (env.IsDevelopment())
            {
                services.AddCustomSwagger();
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
                endpoints.MapCustomHealthChecks("/healthz", $"StorageService - {Guid.NewGuid()}");
                endpoints.MapControllers();
            });

            
        }
    }
}
