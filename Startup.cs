using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace aspnet_spa_without_indexhtml
{
    public class Startup
    {
        private static Action<ISpaBuilder> ConfigureSpaDefaults =
            spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                spa.UseReactDevelopmentServer("start");
            };

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvcWithDefaultRoute();

            if (env.IsDevelopment())
            {
                // Only configure React dev server if in Development
                app.MapWhen(IsSpaRoute, spaApp => {
                    // Only configure React dev server if in Development
                    UseSpaWithoutIndexHtml(spaApp, ConfigureSpaDefaults);
                });
            }
        }

        private static bool IsSpaRoute(HttpContext context)
        {
            var path = context.Request.Path;
            // This should probably be a compiled regex
            return path.StartsWithSegments("/static")
                || path.StartsWithSegments("/sockjs-node")
                || path.StartsWithSegments("/socket.io")
                || path.ToString().Contains(".hot-update.");
        }

        private static void UseSpaWithoutIndexHtml(IApplicationBuilder app, Action<ISpaBuilder> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            // Use the options configured in DI (or blank if none was configured). We have to clone it
            // otherwise if you have multiple UseSpa calls, their configurations would interfere with one another.
            var optionsProvider = app.ApplicationServices.GetService<IOptions<SpaOptions>>();
            var options = new SpaOptions();

            var spaBuilder = new DefaultSpaBuilder(app, options);
            configuration.Invoke(spaBuilder);
        }

        private class DefaultSpaBuilder : ISpaBuilder
        {
            public IApplicationBuilder ApplicationBuilder { get; }

            public SpaOptions Options { get; }

            public DefaultSpaBuilder(IApplicationBuilder applicationBuilder, SpaOptions options)
            {
                ApplicationBuilder = applicationBuilder
                    ?? throw new ArgumentNullException(nameof(applicationBuilder));

                Options = options
                    ?? throw new ArgumentNullException(nameof(options));
            }
        }
    }
}
