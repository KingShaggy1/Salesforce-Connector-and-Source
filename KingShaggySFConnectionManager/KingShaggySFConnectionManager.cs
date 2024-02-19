using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;
using Newtonsoft.Json;

namespace KingShaggySFConnectionManager
{
    [DtsConnection(ConnectionType = "KS Salesforce Connection", DisplayName = "KS Salesforce Connection",
          Description = "Connection Manager for Salesforce",
           ConnectionContact = "King Shaggy Corporation",
   UITypeName = "KingShaggySFConnectionManager.KingShaggySFConnectionManagerUI, KingShaggySFConnectionManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=eaf10663c2c8f9c0"
        )]
    public class KingShaggySFConnectionManager : ConnectionManagerBase
    {
        //private AuthenticationClient auth;
        //private ForceClient client;
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string UserName { get; set; }

        [PasswordPropertyText(true)]
        public string Password { get; set; }
        public string Token { get; set; }
        public string Url { get; set; }
        public string SFApiVersion { get; set; }
        public string InstanceKSUrl { get; set; }
        public string AccessKSToken { get; set; }

        public KingShaggySFConnectionManager()
        {
            //Url = "https://test.salesforce.com/services/oauth2/token";
            //Url = "https://test.salesforce.com";
            //SFApiVersion = "v59.0";

            ClientId = string.Empty;
            ClientSecret = string.Empty;
            UserName = string.Empty;
            Password = string.Empty;
            Token = string.Empty;
            Url = string.Empty;
            SFApiVersion = string.Empty;
        }
        public class AuthConn
        {
            public string AccKSToken { get; set; }
            public string InstKSUrl { get; set; }
            public string ApiKSVersion { get; set; }
        }
        public override object AcquireConnection(object txn)
        {
            var authConn = CreateConnectionAsync();

            if (authConn != null)
            {
                return authConn.Result;
            }
            return null;
        }
        public override void ReleaseConnection(object connection)
        {
            //base.ReleaseConnection(connection);

            if (connection != null)
            {
                var conn = connection as AuthConn;
                //Set conn to null?
            }
        }
        public override DTSExecResult Validate(IDTSInfoEvents infoEvents)
        {
            if (string.IsNullOrWhiteSpace(ClientId))
            {
                return DTSExecResult.Failure;
            }
            else if (string.IsNullOrWhiteSpace(ClientSecret))
            {
                return DTSExecResult.Failure;
            }
            else if (string.IsNullOrWhiteSpace(UserName))
            {
                return DTSExecResult.Failure;
            }
            else if (string.IsNullOrWhiteSpace(Password))
            {
                return DTSExecResult.Failure;
            }
            else if (string.IsNullOrWhiteSpace(Token))
            {
                return DTSExecResult.Failure;
            }
            else if (string.IsNullOrWhiteSpace(Url))
            {
                return DTSExecResult.Failure;
            }
            else if (string.IsNullOrWhiteSpace(SFApiVersion))
            {
                return DTSExecResult.Failure;
            }

            return DTSExecResult.Success;
        }
        public async Task<AuthConn> CreateConnectionAsync()
        {
            AuthConn ac = new AuthConn();

            var isSuccessStatus = AuthenticateAsync().ContinueWith(task => task.GetAwaiter());
            //var isSuccessStatus = await AuthenticateAsync();

            if (isSuccessStatus.Result.GetResult())
            {
                if (!string.IsNullOrEmpty(AccessKSToken) && !string.IsNullOrEmpty(InstanceKSUrl))
                {
                    ac.AccKSToken = AccessKSToken;
                    ac.InstKSUrl = InstanceKSUrl;
                    ac.ApiKSVersion = SFApiVersion;
                }
                else
                {
                    ac.AccKSToken = null;
                    ac.InstKSUrl = null;
                }
            }

            return ac;
        }
        private async Task<bool> AuthenticateAsync()
        {
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls;

            try {
                using (var client = new HttpClient())
                {
                    var tokenEndpoint = $"{Url}/services/oauth2/token";
                    var request = new FormUrlEncodedContent(new[]
                    {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("username", UserName),
                new KeyValuePair<string, string>("password", $"{Password}{Token}")
            });

                    var response = client.PostAsync(tokenEndpoint, request).ContinueWith(task => task.GetAwaiter()).Result.GetResult();
                    //var response = await client.PostAsync(tokenEndpoint, request);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().ContinueWith(task => task.GetAwaiter()).Result.GetResult();

                        AccessKSToken = JsonConvert.DeserializeObject<dynamic>(responseBody).access_token;
                        InstanceKSUrl = JsonConvert.DeserializeObject<dynamic>(responseBody).instance_url;
                    }
                    else
                    {
                        // Handle authentication error
                        Console.WriteLine($"Authentication failed: {response.ReasonPhrase}");
                        MessageBox.Show($"AuthenticationClient failed: {response.ReasonPhrase}", "KingShaggySFConnectionManager", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //return (null, null);
                    }

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Authentication failed: {e.Message}");
                //MessageBox.Show($"AuthenticationClient failed: {e.Message}", "King Shaggy Connection Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
    }
}
