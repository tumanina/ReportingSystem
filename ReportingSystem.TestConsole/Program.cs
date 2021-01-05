using ReportingSystem.ApiClient;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReportingSystem.TestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var client = new HttpClient())
            {
                var healthClient = new HealthClient(client);

                var pingResult = await healthClient.PingAsync();
            }
        }
    }
}
