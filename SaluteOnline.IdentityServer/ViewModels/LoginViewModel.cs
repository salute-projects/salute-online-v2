using System.Collections.Generic;
using System.Linq;
using SaluteOnline.IdentityServer.Domain;

namespace SaluteOnline.IdentityServer.ViewModels
{
    public class LoginViewModel : LoginInputModel
    {
        public bool AllowRememberLogin { get; set; }
        public bool EnableLocalLogin { get; set; }
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }

        public IEnumerable<ExternalProvider> VisibleExternalProviders
            => ExternalProviders.Where(t => !string.IsNullOrEmpty(t.DisplayName));

        public bool IsExternalLoginOnly => !EnableLocalLogin && ExternalProviders?.Count() == 1;
        public string ExternalLoginScheme => ExternalProviders?.SingleOrDefault()?.AuthenticationScheme;

        public bool IsCreateUser { get; set; }
        public RegisterUser RegisterUser { get; set; }
    }
}
