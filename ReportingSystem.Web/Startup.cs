using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ReportingSystem.AzureStorage;
using ReportingSystem.Dal.DbContexts;
using ReportingSystem.Dal.Services;
using ReportingSystem.Logic.Authentification;
using ReportingSystem.Logic.Services;
using ReportingSystem.PowerBI;
using ReportingSystem.Shared.Configuration;
using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Shared.Interfaces.Authentification;
using ReportingSystem.Web.Authentication;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;

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
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });
            services.AddSwaggerGenNewtonsoftSupport();

            var connectionString = Configuration.GetConnectionString("DBConnectionString");
            services.AddDbContext<ReportingDbContext>(options => options.UseSqlServer(connectionString));

            services.Configure<AzureStorageConfiguration>(Configuration.GetSection(nameof(AzureStorageConfiguration)));
            services.Configure<PowerBiConfiguration>(Configuration.GetSection(nameof(PowerBiConfiguration)));
            services.Configure<SecuritySettings>(Configuration.GetSection(nameof(SecuritySettings)));
            services.ConfigureAzureStorageServices();
            services.ConfigurePowerBiServices();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IReportManager, ReportManager>(); 
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAccountDalService, AccountDalService>();
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reporting service");
                c.DocExpansion(DocExpansion.None);
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Reporting service", Version = "v1" });

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header Token. Enter : \"Bearer YourTokenHere\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                var files = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
                foreach (var file in files)
                {
                    c.IncludeXmlComments(file);
                }
            });
        }
    }
}
