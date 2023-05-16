using Microsoft.AspNetCore.Http;

namespace Identity.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static bool ToBool(this string s, bool valueIfNull = true)
        {
            if (s == null)
                return valueIfNull;
            return Convert.ToBoolean(s);
        }

        public static int ToInt(this string s, int valueIfNull = 0)
        {
            if (s == null)
                return valueIfNull;
            return Convert.ToInt32(s);
        }

        public static bool IsValidPath(this PathString path, string route)
        {
            return true;
        }

        public static T ExtractFromRoute<T>(this PathString path, string route)
        {
            return default(T);
        }
    }
}