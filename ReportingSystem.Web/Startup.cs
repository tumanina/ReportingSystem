using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NSwag.Generation.Processors.Security;
using ReportingSystem.AzureStorage;
using ReportingSystem.Dal.DbContexts;
using ReportingSystem.Dal.Services;
using ReportingSystem.Logic.Authentification;
using ReportingSystem.Logic.Managers;
using ReportingSystem.Logic.Services;
using ReportingSystem.PowerBI;
using ReportingSystem.Shared.Configuration;
using ReportingSystem.Shared.Extensions;
using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Shared.Interfaces.Authentification;
using ReportingSystem.Shared.Interfaces.DalServices;
using ReportingSystem.Web.Authentication;
using ReportingSystem.Web.Models;
using System.Collections.Generic;

namespace ReportingSystem.Web
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
            services.AddControllers(config =>
                config.Filters.Add(new UserIdHeaderRequiredFilter()))
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });

            var connectionString = Configuration.GetConnectionString("DBConnectionString");
            services.AddDbContext<ReportingDbContext>(options => options.UseSqlServer(connectionString));

            services.Configure<AzureStorageConfiguration>(Configuration.GetSection(nameof(AzureStorageConfiguration)));
            services.Configure<PowerBiConfiguration>(Configuration.GetSection(nameof(PowerBiConfiguration)));
            services.Configure<SecuritySettings>(Configuration.GetSection(nameof(SecuritySettings)));
            services.ConfigureAzureStorageServices();
            services.ConfigurePowerBiServices();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<ITemplateService, TemplateService>(); 
            services.AddScoped<IReportManager, ReportManager>();
            services.AddScoped<ITemplateManager, TemplateManager>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAccountDalService, AccountDalService>();
            services.AddScoped<IReportDalService, ReportDalService>();
            services.AddScoped<ITemplateDalService, TemplateDalService>();
            services.AddScoped<ITemplateVersionDalService, TemplateVersionDalService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<ISecurityService, Rs256SecurityService>();

            services.ConfigureAuth();

            ConfigureSwagger(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;
                context.Response.StatusCode = 200;
                var message = new BaseApiModel { Errors = new List<string> { exception.GetMessage() } };
                await context.Response.WriteAsync(JsonConvert.SerializeObject(message));
            }));
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Reporting service";
                    document.SecurityDefinitions.Add("ApiKey", new NSwag.OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header Token. Enter : \"Bearer YourTokenHere\"",
                        Name = "Authorization",
                        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                        Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                    });
                };
                config.OperationProcessors.Add(new OperationSecurityScopeProcessor("ApiKey"));
            });
        }
    }
}
