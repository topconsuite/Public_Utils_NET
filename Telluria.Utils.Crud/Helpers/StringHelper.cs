using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Telluria.Utils.Crud.Helpers;

public static class StringHelper
{
  /// <summary>
  ///   Convert first char of string to upper case.
  /// </summary>
  /// <param name="input"> String to convert. </param>
  /// <returns> Converted string. </returns>
  /// <exception cref="ArgumentNullException"> Thrown if input is null. </exception>
  /// <exception cref="ArgumentException"> Thrown if input is empty. </exception>
  public static string FirstCharToUpper(this string input)
  {
    return input switch
    {
      null => throw new ArgumentNullException(nameof(input)),
      "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
      _ => input.First().ToString().ToUpper() + input[1..]
    };
  }

  /// <summary>
  ///   Convert string to pascal case.
  /// </summary>
  /// <param name="source"> String to convert. </param>
  /// <returns> Converted string. </returns>
  public static string ToPascalCase(this string source)
  {
    if (source == null || source.Length == 0) return "";

    var words = source.Split('_')
      .Select(t =>
      {
        if (t.Length == 0) return "";
        var firtChar = char.ToUpperInvariant(t[0]).ToString();
        if (t.Length == 1) return firtChar;
        return firtChar + t[1..];
      });

    return string.Concat(words);
  }

  /// <summary>
  ///   Convert string to camel case.
  /// </summary>
  /// <param name="source"> String to convert. </param>
  /// <returns> Converted string. </returns>
  public static string ToCamelCase(this string source)
  {
    if (source == null || source.Length == 0) return "";
    var pascalCase = source.ToPascalCase();
    var firtChar = char.ToLowerInvariant(pascalCase[0]).ToString();
    if (pascalCase.Length == 1) return firtChar;
    return firtChar + pascalCase[1..];
  }

  /// <summary>
  ///   Convert string to snake case.
  /// </summary>
  /// <param name="source"> String to convert. </param>
  /// <returns> Converted string. </returns>
  public static string ToSnakeCase(this string source)
  {
    if (source == null || source.Length == 0) return "";

    return string.Concat(
      source.Select(
        (x, i) => (i > 0 && char.IsUpper(x) ? "_" : "") + char.ToLowerInvariant(x)
      )
    );
  }

  /// <summary>
  ///   Verify if string is numeric.
  /// </summary>
  /// <param name="text"> String to verify. </param>
  /// <returns> True if string is numeric, false otherwise. </returns>
  public static bool IsNumeric(this string text)
  {
    return double.TryParse(text, out _);
  }

  /// <summary>
  ///   Remove accents from string.
  /// </summary>
  /// <param name="input"> String to remove accents. </param>
  /// <returns> String without accents. </returns>
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

  /// <summary>
  ///   Verify if string contains another string ignoring case and accents.
  /// </summary>
  /// <param name="input"> String to verify. </param>
  /// <param name="searchValue"> String to search. </param>
  /// <returns> True if string contains another string ignoring case and accents, false otherwise. </returns>
  public static bool ContainsIgnoreCaseAndAccents(this IEnumerable<string> input, string searchValue)
  {
    return input.Contains(searchValue, IgnoreCaseAndAccentsEqualityComparer.Instance);
  }

  /// <summary>
  ///   Verify if string equals another string ignoring case and accents.
  /// </summary>
  /// <param name="a"> String to verify. </param>
  /// <param name="b"> String to compare. </param>
  /// <returns> True if string equals another string ignoring case and accents, false otherwise. </returns>
  public static bool EqualsIgnoreCaseAndAccents(this string a, string b)
  {
    return a.RemoveAccents().Equals(b.RemoveAccents(), StringComparison.InvariantCultureIgnoreCase);
  }
}

public sealed class IgnoreCaseAndAccentsEqualityComparer : IEqualityComparer<string>
{
  public static IEqualityComparer<string> Instance { get; } = new IgnoreCaseAndAccentsEqualityComparer();

  public bool Equals(string x, string y)
  {
    return x.EqualsIgnoreCaseAndAccents(y);
  }

  public int GetHashCode(string obj)
  {
    return obj.GetHashCode();
  }
}
