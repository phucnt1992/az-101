using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Az101.Gallery.Infrastructure.Extensions;

using Az101.Gallery.Infrastructure.HealthCheck;


namespace Az101.Gallery;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();

        // In production, the React files will be served from this directory
        services.AddSpaStaticFiles(configuration =>
        {
            configuration.RootPath = "ClientApp/build";
        });

        services
            .AddDbServices(Configuration)
            .AddStorageServices(Configuration)
            .AddHealthCheckServices();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider service)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSpaStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions()
            {
                ResponseWriter = ResponseWriter.WriteResponse
            });

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");
        });

        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = "ClientApp";

            if (env.IsDevelopment())
            {
                spa.UseReactDevelopmentServer(npmScript: "start");
            }
        });
        app.EnsureDependencyServices(service);
    }

}

