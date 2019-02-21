using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GABDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GABDemo
{
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<ComputerVisionOptions>(Configuration.GetSection("ComputerVision"))
                    .PostConfigure<ComputerVisionOptions>(options =>
                    {
                        if (string.IsNullOrEmpty(options.ApiKey))
                        {
                            throw new Exception("Computer Vision API Key is missing");
                        }

                        if (string.IsNullOrEmpty(options.ApiEndPoint))
                        {
                            throw new Exception("Computer Vision API Key is missing");
                        }

                    });
            services.Configure<StorageAccountOptions>(options =>
            {
                options.ConnectionString = Configuration.GetConnectionString("ApplicationStorage");
            });

            services.AddScoped<IImageAnalyzer, ImageAnalyzer>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ComputerVisionOptions>>().Value;
                var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(options.ApiKey)) { Endpoint = options.ApiEndPoint };
                return new ImageAnalyzer(client);
            });
            services.AddScoped<IBlogStorageManager, BlobStorageManager>(sp => new BlobStorageManager(sp.GetRequiredService<IOptions<StorageAccountOptions>>().Value));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
