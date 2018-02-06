using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using SaluteOnline.IdentityServer.Constants;
using SaluteOnline.IdentityServer.Domain;
using SaluteOnline.IdentityServer.ViewModels;

namespace SaluteOnline.IdentityServer.Service.Implementation
{
    public class AccountService
    {
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IClientStore _clientStore;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticationSchemeProvider _schemeProvider;

        public AccountService(IIdentityServerInteractionService interactionService,
            IHttpContextAccessor httpContextAccessor, IAuthenticationSchemeProvider schemeProvider,
            IClientStore clientStore)
        {
            _interactionService = interactionService;
            _clientStore = clientStore;
            _httpContextAccessor = httpContextAccessor;
            _schemeProvider = schemeProvider;
        }

        public async Task<LoginViewModel> GenerateLoginViewModelAsync(string returnUrl)
        {
            var context = await _interactionService.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null)
            {
                return new LoginViewModel
                {
                    EnableLocalLogin = false,
                    ReturnUrl = returnUrl,
                    Username = context.LoginHint,
                    ExternalProviders = new List<ExternalProvider> { new ExternalProvider { AuthenticationScheme = context.IdP } }
                };
            }
            var schemes = await _schemeProvider.GetAllSchemesAsync();
            var providers =
                schemes.Where(t => !string.IsNullOrEmpty(t.DisplayName)).Select(t => new ExternalProvider
                {
                    DisplayName = t.DisplayName,
                    AuthenticationScheme = t.Name
                }).ToList();
            var result = new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };

            if (string.IsNullOrEmpty(context?.ClientId))
                return result;

            var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
            if (client == null)
                return result;
            if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
            {
                providers =
                    providers.Where(t => client.IdentityProviderRestrictions.Contains(t.AuthenticationScheme))
                        .ToList();
            }
            result.EnableLocalLogin = client.EnableLocalLogin && AccountOptions.AllowLocalLogin;
            result.ExternalProviders = providers.ToArray();
            return result;
        }

        public async Task<LoginViewModel> GenerateLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await GenerateLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        public async Task<LogoutInputModel> GenerateLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel
            {
                LogoutId = logoutId,
                ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt
            };
            var user = _httpContextAccessor.HttpContext.User;
            if (user?.Identity.IsAuthenticated != true)
            {
                vm.ShowLogoutPrompt = false;
                return vm;
            }
            var context = await _interactionService.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt != false)
                return vm;

            vm.ShowLogoutPrompt = false;
            return vm;
        }

        public async Task<LoggedOutViewModel> GenerateLoggoutOutViewModelAsync(string logoutId)
        {
            var logout = await _interactionService.GetLogoutContextAsync(logoutId);
            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                SignoutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };
            var user = _httpContextAccessor.HttpContext.User;
            if (user?.Identity.IsAuthenticated != true)
                return vm;

            var idp = user.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            if (idp == null || idp == IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                return vm;

            var providerSupportsSignout = await _httpContextAccessor.HttpContext.GetSchemeSupportsSignOutAsync(idp);
            if (!providerSupportsSignout)
                return vm;

            if (vm.LogoutId == null)
            {
                vm.LogoutId = await _interactionService.CreateLogoutContextAsync();
            }
            vm.ExternalAuthenticationScheme = idp;
            return vm;
        }
    }
}
