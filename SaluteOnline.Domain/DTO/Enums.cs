namespace SaluteOnline.Domain.DTO
{
    public enum Role
    {
        None = 0,
        User = 1,
        ClubAdmin = 2,
        SuperAdmin = 3
    }

    public enum ActivityType
    {
        SignUp = 0,
        Login = 1,
        ForgotPassword = 2
    }

    public enum ActivityImportance
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }

    public enum Roles
    {
        None = 0,
        User = 1,
        ClubAdmin = 2,
        GlobalAdmin = 3,
        SilentDon = 4
    }

    public enum Policies
    {
        User = 0,
        ClubAdmin = 1,
        GlobalAdmin = 2,
        SilendDon = 5
    }
}
