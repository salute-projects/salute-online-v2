using System;

namespace SaluteOnline.IdentityServer.Constants
{
    public class AccountOptions
    {
        public static bool AllowLocalLogin = true;
        public static bool AllowRememberLogin = true;
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
        public static bool ShowLogoutPrompt = true;
        public static bool AutomaticRedirectAfterSignOut = false;
        public static bool WindowsAuthenticationEnabled = true;
        public static bool IncludeWindowsGroups = false;
        public static readonly string WindowsAuthenticationSchemeName = "Windows";
        public static string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}
