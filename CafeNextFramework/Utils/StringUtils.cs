using System.Text.RegularExpressions;

namespace CafeNextFramework.Utilities
{
    public static class StringUtilities
    {
        public static Regex Pattern(string value, bool ignoreCase)
        {
            return (ignoreCase) ? new Regex(value, RegexOptions.IgnoreCase) : new Regex(value);
        }

        public static bool Match(string valueToBeMatched, Regex pattern)
        {
            return (!string.IsNullOrEmpty(valueToBeMatched) && pattern != null) && pattern.Match(valueToBeMatched).Success;
        }
    }
}