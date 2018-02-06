namespace SaluteOnline.IdentityServer.ViewModels
{
    public class LoggedOutViewModel
    {
        public string PostLogoutRedirectUri { get; set; }
        public string ClientName { get; set; }
        public string SignoutIframeUrl { get; set; }
        public bool AutomaticRedirectAfterSignOut { get; set; }
        public string LogoutId { get; set; }
        public string ExternalAuthenticationScheme { get; set; }
        public bool TriggerExternalSignout => !string.IsNullOrEmpty(ExternalAuthenticationScheme);
    }
}
