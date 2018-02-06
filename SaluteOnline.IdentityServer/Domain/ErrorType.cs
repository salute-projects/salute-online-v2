namespace SaluteOnline.IdentityServer.Domain
{
    public enum ErrorType
    {
        Unknown = 0,
        LoginFail = 1,
        RegisterFail = 2,
        ExternalLoginFail = 3,
        LogoutFail = 4,
        ConfirmEmailFail = 5,
        ChangePasswordFail = 6,
        ResetPasswordFail = 7,
        ForgotPasswordFail = 8,
        ContactSupportFail = 9
    }
}