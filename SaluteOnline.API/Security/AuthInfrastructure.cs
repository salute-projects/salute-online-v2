using System;
using Jwt;
using Newtonsoft.Json;

namespace SaluteOnline.API.Security
{
    public class AuthInfrastructure
    {
        private const string AuthPrefix = "auth0|";

        public static AuthPayload Decode(string authorization, string issuerUrl)
        {
            var token = ClearToken(authorization);
            var payload = ExtractPayload(token, issuerUrl);
            return payload;
        }

        private static string ClearToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            if (token.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
                token = token.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(token))
                throw new Exception("Authorization token value is empty");
            return token;
        }

        private static AuthPayload ExtractPayload(string token, string issuerUrl)
        {
            var payload = DecodePayload(token);
            if (IsTokenValid(payload, issuerUrl))
                return payload;
            throw new Exception("Authorization Token is invalid");

        }

        private static AuthPayload DecodePayload(string token)
        {
            var result = JsonWebToken.Decode(token, string.Empty, false);
            var payload = JsonConvert.DeserializeObject<AuthPayload>(result);
            return payload;
        }

        private static bool IsTokenValid(AuthPayload payload, string issuerUrl)
        {
            return !string.IsNullOrEmpty(payload?.Subject) && !string.IsNullOrEmpty(payload.Issuer) &&
                   payload.Issuer.Contains(issuerUrl) && payload.Subject.StartsWith(AuthPrefix);
        }
    }
}
