using Covid19Nz.Actions;
using Covid19Nz.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace Covid19Nz
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddScoped<IScraperActions, ScraperActions>();

                var sources = Configuration.GetSection(nameof(Sources)).Get<Sources>();

                services.AddSingleton(sources);

                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Covid19Nz", Version = "v1" });
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseMvc();

                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Covid-19 Nz API V1");
                    c.RoutePrefix = string.Empty;
                });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
