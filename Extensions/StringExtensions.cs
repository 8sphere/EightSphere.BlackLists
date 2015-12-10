using System.Text.RegularExpressions;

namespace EightSphere.BlackLists.Services
{
    internal static class StringExtensions
    {
        public static string EscapeRegex(this string input)
        {
            return Regex.Replace(input, @"[\-\[\]\/\{\}\(\)\+\?\.\\\^\$\|]", @"\$&");
        }
    }
}