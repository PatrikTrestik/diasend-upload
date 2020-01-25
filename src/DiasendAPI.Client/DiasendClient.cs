using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiasendAPI
{    
    /// <summary>
    /// Diasend API cleint wrapper. High level methods.
    /// </summary>
    public class DiasendClient
    {
        private readonly string username;
        private readonly string password;
        private readonly Guid randomUuid = new Guid("0A645BC8-C18F-4A4C-8C5C-24F68B804C89");
        private readonly Guid glucoseUuid = new Guid("00001808-0000-1000-8000-00805f9b34fb");

        /// <summary>
        /// Initialize client.
        /// </summary>
        /// <param name="username">Diasend servers User id</param>
        /// <param name="password">Diasend servers User password</param>
        public DiasendClient(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
        /// <summary>
        /// Reads registered/known 'devices'
        /// </summary>
        /// <returns>List of Device</returns>
        public async Task<IEnumerable<Device>> GetRegisteredDevices()
        {
            using (var client = new Client(username, password))
            {
                client.BaseUrl = "https://api.diasend.com/1/";
                var data = await client.GetPatientSummaryAsync();
                return data.Select(d=>d.Device);
            }
        }
        /// <summary>
        /// Reads all patient data between dates. Number of results is server limited, keep interval small.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DataItem>> GetPatientData(DateTimeOffset from, DateTimeOffset to)
        {
            using(var client = new Client(username,password))
            {
                client.BaseUrl = "https://api.diasend.com/1/";
                var data= await client.GetPatientDataAsync(Type.Combined, null, from, to);
                return data;
            }
        }
        /// <summary>
        /// Uploads reading to server
        /// </summary>
        /// <param name="payload">Raw byte array. GATT??</param>
        /// <returns></returns>
        public async Task UploadGlucoseData(byte[] payload)
        {
            using (var client = new Client(username, password))
            {
                client.BaseUrl = "https://api.diasend.com/1/";
                var data = new DataUpload()
                {
                    Device_type = "GATTProfileGlucose",                   
                    Pcode = username,
                    Password = password,
                    Uuid = randomUuid.ToString("n").Substring(0,16),
                    Payload = Convert.ToBase64String(payload)
                };
                await client.PostPatientDataAsync(data);
            }
        }
    }
}
