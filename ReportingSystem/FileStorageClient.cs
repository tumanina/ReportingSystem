using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ReportingSystem
{
    internal class FileStorageClient : IFileStorageClient
    {
        private string _token;
        private string _url;

        public void ConfigureClient(string token, string url)
        {
            _token = token;
            _url = url;
        }

        public async Task<IEnumerable<string>> GetFileNames()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                HttpResponseMessage response = await client.GetAsync($"{_url}/files");
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<IEnumerable<string>>(content);
                }
                else
                {
                    throw new Exception("Failed");
                }
            }
        }
    }
}
