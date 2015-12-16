using System;
using System.Text.RegularExpressions;

namespace EightSphere.BlackLists.Extensions
{
    internal static class StringExtensions
    {
        public static string EscapeRegex(this string input)
        {
            return Regex.Replace(input, @"[\-\[\]\/\{\}\(\)\+\?\.\\\^\$\|]", @"\$&");
        }
    }
}