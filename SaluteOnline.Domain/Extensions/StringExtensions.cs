namespace SaluteOnline.Domain.Extensions
{
    public static class StringExtensions
    {
        public static string ToLowerString<T>(this T value)
        {
            return value.ToString().ToLowerInvariant();
        }
    }
}
