using System.Collections.Generic;
using System.Runtime.Serialization;
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

            // Blob Storage
            services.AddOptions<StorageAccountOptions>()
                .Configure(options => options.ConnectionString = Configuration.GetConnectionString("ApplicationStorage"))
                .ValidateDataAnnotations();
            services.AddSingleton<IBlobStorageManager, BlobStorageManager>();

            // Computer Vision
            services.AddOptions<ComputerVisionOptions>()
                .Bind(Configuration.GetSection("ComputerVision"))
                .ValidateDataAnnotations();
            services.AddSingleton(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<ComputerVisionOptions>>().Value;
                return new ComputerVisionClient(new ApiKeyServiceClientCredentials(options.ApiKey)) { Endpoint = options.ApiEndPoint };
            });
            services.AddSingleton<IImageAnalyzer, ImageAnalyzer>();

            // MVC
            services
                .AddMvc(options => options.Filters.Add<OptionsValidationExceptionFilterAttribute>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
            app.UseMvcWithDefaultRoute();
        }
    }
}
