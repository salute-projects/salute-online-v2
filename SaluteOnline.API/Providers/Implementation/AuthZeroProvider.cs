using System;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Auth0.Core;
using Auth0.ManagementApi;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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

        private ManagementApiClient CreateManagementClient()
        {
            using (var client = CreateClient())
            {
                dynamic parameter = new ExpandoObject();
                parameter.client_id = _settings.ClientId;
                parameter.client_secret = _settings.ClientSecret;
                parameter.grant_type = "client_credentials";
                parameter.audience = _settings.Audience;
                var requestBody = JsonConvert.SerializeObject(parameter);
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                var result = client.PostAsync("/oauth/token", content).Result;
                var resultContent = result.Content.ReadAsStringAsync().Result;
                var securityToken = JsonConvert.DeserializeObject<SecurityToken>(resultContent);
                return new ManagementApiClient(securityToken.AccessToken, new Uri(_settings.Audience));
            }
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
                parameter.scope = "offline_access";
                parameter.connection = "Username-Password-Authentication";
                var requestBody = JsonConvert.SerializeObject(parameter);
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                var result = await client.PostAsync("/oauth/token", content);
                var resultContent = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<LoginResponse>(resultContent);
            }
        }

        public async Task<LoginResponse> RefreshToken(string refreshToken)
        {
            using (var client = CreateClient())
            {
                dynamic parameter = new ExpandoObject();
                parameter.grant_type = "refresh_token";
                parameter.audience = _settings.Audience;
                parameter.client_id = _settings.ClientId;
                parameter.client_secret = _settings.ClientSecret;
                parameter.refresh_token = refreshToken;
                parameter.connection = "Username-Password-Authentication";
                var requestBody = JsonConvert.SerializeObject(parameter);
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                var result = await client.PostAsync("/oauth/token", content);
                var resultContent = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<LoginResponse>(resultContent);
            }
        }

        public async Task<string> RunForgotPasswordFlow(string email)
        {
            using (var client = CreateClient())
            {
                dynamic parameter = new ExpandoObject();
                parameter.client_id = _settings.ClientId;
                parameter.email = email;
                parameter.connection = "Username-Password-Authentication";
                var requestBody = JsonConvert.SerializeObject(parameter);
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                var result = await client.PostAsync("/dbconnections/change_password", content);
                return await result.Content.ReadAsStringAsync();
            }
        }

        public async Task<User> GetUserById(string userId)
        {
            var client = CreateManagementClient();
            var user = await client.Users.GetAsync(userId);
            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var client = CreateManagementClient();
            var users = await client.Users.GetAllAsync(q: "email:\"" + email + "\"");
            return users.Single();
        }

        public Task<User> GetUserByToken(string token)
        {
            var payload = AuthInfrastructure.Decode(token, _settings.Domain);
            var userId = payload.Subject;
            return GetUserById(userId);
        }
    }
}
