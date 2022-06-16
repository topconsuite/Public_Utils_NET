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
      return input switch
      {
        null => throw new ArgumentNullException(nameof(input)),
        "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
        _ => input.First().ToString().ToUpper() + input[1..],
      };
    }

    public static string ToPascalCase(this string source)
    {
      if (source == null || source.Length == 0) return "";

      var words = source.Split('_').Select(t =>
      {
        if (t.Length == 0) return "";
        var firtChar = char.ToUpperInvariant(t[0]).ToString();
        if (t.Length == 1) return firtChar;
        return firtChar + t[1..];
      });

      return string.Concat(words);
    }

    public static string ToCamelCase(this string source)
    {
      if (source == null || source.Length == 0) return "";
      var pascalCase = source.ToPascalCase();
      var firtChar = char.ToLowerInvariant(pascalCase[0]).ToString();
      if (pascalCase.Length == 1) return firtChar;
      return firtChar + pascalCase[1..];
    }

    public static string ToSnakeCase(this string source)
    {
      if (source == null || source.Length == 0) return "";

      return string.Concat(
          source.Select(
            (x, i) => (i > 0 && char.IsUpper(x) ? "_" : "") + char.ToLowerInvariant(x)
          )
        );
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
      unchecked
      {
        return obj.GetHashCode();
      }
    }
  }
}
