using SaluteOnline.Shared.Common;

namespace SaluteOnline.Shared.Helpers
{
    public static class RolesHelper
    {
        public static Roles ParseRole(string role)
        {
            switch (role.ToLowerInvariant())
            {
                case "user":
                    return Roles.User;
                case "clubadmin":
                    return Roles.ClubAdmin;
                case "globaladmin":
                    return Roles.GlobalAdmin;
                case "silentdon":
                    return Roles.SilentDon;
                case "guest":
                    return Roles.Guest;
                default:
                    return Roles.None;
            }
        }
    }
}
