using Microsoft.Extensions.DependencyInjection;
using ReportingSystem.PowerBI.Interfaces;
using ReportingSystem.Shared.Interfaces;

namespace ReportingSystem.PowerBI
{
    public static class PowerBiExtensions
    {
        public static void ConfigurePowerBiServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IReportEngineTool, PowerBiService>();
        }
    }
}
