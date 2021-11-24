using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Telluria.Utils.Crud.Helpers
{
    public static class StringHelper
    {
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        public static bool IsNumeric(this string text) => double.TryParse(text, out _);

        public static string RemoveAccents(this string input)
        {
            var normalizedString = input.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < normalizedString.Length; i++)
            {
                var c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        public static bool ContainsIgnoreCaseAndAccents(this IEnumerable<string> input, string searchValue)
        {
            return input.Contains(searchValue, IgnoreCaseAndAccentsEqualityComparer.Instance);
        }

        public static bool EqualsIgnoreCaseAndAccents(this string a, string b)
        {
            return a.RemoveAccents().Equals(b.RemoveAccents(), StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public sealed class IgnoreCaseAndAccentsEqualityComparer : IEqualityComparer<string>
    {
        private static readonly IEqualityComparer<string> _instance = new IgnoreCaseAndAccentsEqualityComparer();
        public static IEqualityComparer<string> Instance { get => _instance; }

        public bool Equals(string x, string y)
        {
            return x.EqualsIgnoreCaseAndAccents(y);
        }

        public int GetHashCode(string obj)
        {
            unchecked { return obj.GetHashCode(); };
        }

        
    }
}
