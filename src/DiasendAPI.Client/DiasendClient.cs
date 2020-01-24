using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiasendAPI
{
    public class DiasendClient
    {
        private readonly string username;
        private readonly string password;

        public DiasendClient(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public async Task<IEnumerable<Device>> GetRegisteredDevices()
        {
            using (var client = new Client(username, password))
            {
                client.BaseUrl = "https://api.diasend.com/1/";
                var data = await client.GetPatientSummaryAsync();
                return data.Select(d=>d.Device);
            }
        }

        public async Task<IEnumerable<DataItem>> GetPatientData(DateTimeOffset from, DateTimeOffset to)
        {
            using(var client = new Client(username,password))
            {
                client.BaseUrl = "https://api.diasend.com/1/";
                var data= await client.GetPatientDataAsync(Type.Combined, null, from, to);
                return data;
            }
        }
    }
}
