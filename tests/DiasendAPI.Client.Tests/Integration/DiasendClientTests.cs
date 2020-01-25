using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using DiasendAPI;
using System.Threading.Tasks;
using System.Linq;

namespace DiasendAPI.Client.Tests.Integration
{
    [TestClass]
    public class DiasendClientTests
    {
        public DiasendClientTests()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<DiasendClientTests>();

            Configuration = builder.Build();
        }

        IConfiguration Configuration { get; set; }

        [TestCategory("Integration API test")]
        [TestMethod]
        public async Task ReadData()
        {
            var username = Configuration["ApiUsername"];
            var password = Configuration["ApiPassword"];
            var testSerial = "SM30226252"; //Change this to serial of your your device

            var client = new DiasendClient(username,password);

            var devices = await client.GetRegisteredDevices();
            Assert.IsNotNull(devices);
            Assert.IsTrue(devices.Any(d => d.Serial == username)); //There is always listed user among the devices
            var device = devices.FirstOrDefault(d => d.Serial == testSerial);
            Assert.IsNotNull(device);
            var lastData = device.Last_value_at;
            Assert.IsNotNull(lastData);            

            var data=await client.GetPatientData(lastData.AddDays(-10), lastData);
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Any());
        }

        [TestCategory("Integration API test")]
        [TestMethod]
        public async Task UploadData()
        {
            var username = Configuration["ApiUsername"];
            var password = Configuration["ApiPassword"];
            var sequence = BitConverter.GetBytes(0); //Sequence number ?? 2 bytes
            byte unknown2 = 0x00;
            var testDate = DateTime.Now;
            var year = BitConverter.GetBytes(testDate.Year); //Not sure if correct as LittleEndian
            var month = BitConverter.GetBytes(testDate.Month);
            var day = BitConverter.GetBytes(testDate.Day);
            var hour = BitConverter.GetBytes(testDate.Hour);
            var minute = BitConverter.GetBytes(testDate.Minute);
            var second = BitConverter.GetBytes(testDate.Second);
            var bGmolL = Half.GetBytes(new Half(9.5));
            var testPayload = new byte[] { 
                0x00, 0x00, // 0- leading 2b
                0x0B, // 2 -flags, 0x01 - time offset present, 0x02 - glucose data present, 0x04 - mol/l, 0x08 - sensor status present, 0x10 - context info follows
                sequence[0], sequence[1], // 3 - sequence 2b 
                year[0], year[1], // 5
                month[0], // 6
                day[0], // 7
                hour[0], // 8
                minute[0], // 9
                second[0], // 10
                0x01, 0x00, // 11 - time offset 2b, presaent if flags0=1
                bGmolL[0], bGmolL[1], // 13 - sFloat 2b ??, present if flags1=1, unit  flags2= 0 (kg/L), 1 (mol/l)
                0x00, // 15 - sample location and type ??
                0xF8, // 16 - status ??
            };

            var client = new DiasendClient(username, password);

            //Status: 500
            //Response: { "error":"invalid_request","error_description":"Internal terminal serial failure."}
            try
            {
                await client.UploadGlucoseData(testPayload);
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            
        }

    }
}
