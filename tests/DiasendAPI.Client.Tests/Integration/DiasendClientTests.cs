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

        [TestMethod]
        public async Task ReadPatientDevices()
        {
            var username = Configuration["ApiUsername"];
            var password = Configuration["ApiPassword"];
            var testSerial = Configuration["DeviceSerial"]; //Change this to serial of your your device

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

    }
}
