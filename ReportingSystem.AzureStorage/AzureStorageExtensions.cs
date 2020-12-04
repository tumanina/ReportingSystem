using Microsoft.Extensions.DependencyInjection;
using ReportingSystem.Shared.Interfaces;

namespace ReportingSystem.AzureStorage
{
    public static class AzureStorageExtensions
    {
        public static void ConfigureAzureStorageServices(this IServiceCollection services)
        {
            services.AddScoped<IFileStorage, AzureFileStorage>();
        }
    }
}
