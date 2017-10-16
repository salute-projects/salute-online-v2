using System;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using SaluteOnline.API.Providers.Interface;
using SaluteOnline.API.Security;
using SaluteOnline.Domain.DTO.Auth0;
using SaluteOnline.Domain.DTO.User;

namespace SaluteOnline.API.Providers.Implementation
{
    public class AuthZeroProvider : IAuthZeroProvider
    {
        private readonly Auth0Settings _settings;

        public AuthZeroProvider(IOptions<Auth0Settings> settings)
        {
            _settings = settings.Value;
        }

        private static HttpClient CreateClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://saluteonline.eu.auth0.com/")
            };
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public async Task<SignUpResponse> SignUp(UserEssential user)
        {
            using (var client = CreateClient())
            {
                dynamic parameter = new ExpandoObject();
                parameter.client_id = _settings.ClientId;
                parameter.email = user.Email;
                parameter.password = user.Password;
                parameter.connection = "Username-Password-Authentication";
                dynamic metadata = new ExpandoObject();
                metadata.role = "user";
                parameter.user_metadata = metadata;
                var requestBody = JsonConvert.SerializeObject(parameter);
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                var result = await client.PostAsync("/dbconnections/signup", content);
                var resultContent = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<SignUpResponse>(resultContent);
            }
        }

        public async Task<LoginResponse> GetToken(UserEssential user)
        {
            using (var client = CreateClient())
            {
                dynamic parameter = new ExpandoObject();
                parameter.grant_type = "password";
                parameter.username = user.Email;
                parameter.password = user.Password;
                parameter.audience = _settings.Audience;
                parameter.client_id = _settings.ClientId;
                parameter.client_secret = _settings.ClientSecret;
                parameter.connection = "Username-Password-Authentication";
                var requestBody = JsonConvert.SerializeObject(parameter);
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                var result = await client.PostAsync("/oauth/token", content);
                var resultContent = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<LoginResponse>(resultContent);
            }
        }
    }
}
