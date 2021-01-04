using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ReportingSystem
{
    public class FileStorageClient : IFileStorageClient
    {
        private string _url;
        private string _token;

        public void ConfigureClient(string url, string token)
        {
            _url = url;
            _token = token;
        }

        public async Task<BaseApiDataModel<IEnumerable<string>>> GetFileNames()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                HttpResponseMessage response = await client.GetAsync($"{_url}/files");
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<BaseApiDataModel<IEnumerable<string>>>(content);
                }
                else
                {
                    throw new Exception("Failed");
                }
            }
        }
    }
}
