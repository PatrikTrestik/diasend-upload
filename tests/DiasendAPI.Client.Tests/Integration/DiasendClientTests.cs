using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using DiasendAPI;
using System.Threading.Tasks;
using System.Linq;
using DiasendAPI.GATGlucoseMeasurement;

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

        [TestCategory("Integration API test")]
        [TestMethod]
        public async Task UploadData()
        {
            var username = Configuration["ApiUsername"];
            var password = Configuration["ApiPassword"];
            var flags = (byte)(GlucoseMeasurementFlags.GlucoseConcentrationUnits | GlucoseMeasurementFlags.GlucoseTypeSampleLocationPresent);
            var sequence = BitConverter.GetBytes(4); //Sequence number ?? 2 bytes
            byte unknown2 = 0x00;
            var testDate = DateTime.Now;
            var year = BitConverter.GetBytes(testDate.Year); //Not sure if correct as LittleEndian
            var month = BitConverter.GetBytes(testDate.Month);
            var day = BitConverter.GetBytes(testDate.Day);
            var hour = BitConverter.GetBytes(testDate.Hour);
            var minute = BitConverter.GetBytes(testDate.Minute);
            var second = BitConverter.GetBytes(testDate.Second);
            var bGmolL = Half.GetBytes(new Half(9.5));
            var typeAndLocation = (byte)(((byte)GlucoseSampleLocation.Finger << 4) + (byte)GlucoseSampleType.CapillaryWholeBlood);

            var payload = new List<byte>();
            payload.AddRange(System.Text.UTF8Encoding.UTF8.GetBytes($"!TAG!SEPARATOR!TAG!SERIAL!{Configuration["DeviceSerial"]}!TAG!SEPARATOR!TAG!MODEL!929!TAG!SEPARATOR!TAG!NAME!meter+06169505!TAG!SEPARATOR!TAG!MANUFACTURER!Roche!TAG!SEPARATOR!TAG!HWREV!!TAG!SEPARATOR!TAG!FWREV!v1.8.6!TAG!SEPARATOR!TAG!SWREV!!TAG!SEPARATOR!TAG!SYSID!"));
            payload.AddRange(new byte[] { 0xA1, 0x23, 0x67, 0x01, 0x00, 0x19, 0x60, 0x00 });
            payload.AddRange(System.Text.UTF8Encoding.UTF8.GetBytes("!TAG!SEPARATOR!TAG!PNPID!"));
            payload.AddRange(new byte[] { 0x01, 0x70, 0x01, 0xD5, 0x21, 0x72, 0x01 });
            payload.AddRange(System.Text.UTF8Encoding.UTF8.GetBytes("!TAG!SEPARATOR!TAG!CERTDATA!!TAG!SEPARATOR!TAG!VALUES!"));
            payload.AddRange(new byte[] {
                flags, // 2 -flags, 0x01 - time offset present, 0x02 - glucose data present, 0x04 - mol/l, 0x08 - sensor status present, 0x10 - context info follows
                sequence[0], sequence[1], // 3 - sequence 2b 
                year[0], year[1], // 5
                month[0], // 6
                day[0], // 7
                hour[0], // 8
                minute[0], // 9
                second[0], // 10               
                bGmolL[0], bGmolL[1], // 13 - sFloat 2b, present if flags1=1, unit  flags2= 0 (kg/L), 1 (mol/l)
                typeAndLocation, //Location+Type
            });
            payload.AddRange(System.Text.UTF8Encoding.UTF8.GetBytes("!TAG!SEPARATOR!TAG!FLAGS!"));
            var client = new DiasendClient(username, password);

            //Status: 500
            //Response: { "error":"invalid_request","error_description":"Internal terminal serial failure."}
            try
            {
                await client.UploadGlucoseData(payload.ToArray());
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            
        }

    }
}
