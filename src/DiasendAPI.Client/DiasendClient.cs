using System;
using System.Net.Http;

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
        public void GetPatientData()
        {
            using(var client = new Client(username,password))
            {
                client.BaseUrl = "https://api.diasend.com/1/";
                client.DataAllAsync("", null, null, null);
            }
        }
    }
}
