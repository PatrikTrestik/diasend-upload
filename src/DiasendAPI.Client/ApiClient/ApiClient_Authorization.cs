using IdentityModel.Client;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DiasendAPI
{
    /// <summary>
    /// Partial. Část přidává Basic autorizaci.
    /// </summary>
    public partial class Client : IDisposable
    {
        private readonly string _userName;
        private readonly string _password;

        /// <summary>
        /// Gets the retrieve authorization token.
        /// </summary>
        public Func<Task<string>> RetrieveAuthorizationToken { get; }

        /// <summary>
        /// OAuth2 authorization
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="password">password</param>
        public Client(string userName, string password) : this(new System.Net.Http.HttpClient())
        {
            _userName = userName;
            _password = password;
            RetrieveAuthorizationToken = GetAccessToken;
        }

        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            if (RetrieveAuthorizationToken != null)
            {
                var token = RetrieveAuthorizationToken().Result;
                request.SetBearerToken(token);
            }            
        }

        private async Task<string> GetAccessToken()
        {
            var tokenResponse = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = $"{BaseUrl?.TrimEnd('/')??""}/oauth2/token",
                ClientId = "a486o3nvdu88cg0sos4cw8cccc0o0cg.api.diasend.com",
                ClientSecret = "8imoieg4pyos04s44okoooowkogsco4",
                UserName= _userName,
                Password= _password,
                Scope = "PATIENT DIASEND_MOBILE_DEVICE_DATA_RW"
            });

            if (tokenResponse.IsError)
            {
                throw new UnauthorizedAccessException("Login failed", tokenResponse.Exception);
            }

            return tokenResponse.AccessToken;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_httpClient != null) _httpClient.Dispose();
            }
        }
    }
}
