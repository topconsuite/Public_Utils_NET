using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Telluria.Utils.Crud.Helpers;

namespace Telluria.Utils.Crud.QueryFilters;

public static class ExpressionParser
{
  // string pattern definitions
  private static readonly string _startPattern = "$(";
  private static readonly string _endPattern = ")";
  private static readonly char _separatorPattern = ';';
  private static readonly char _propertySeparatorPattern = '|';
  private static readonly char _inSeparatorPattern = '|';

  private static readonly Dictionary<string, string> _possibleOperations = new(new[]
  {
    new KeyValuePair<string, string>("==", "Equal"),
    new KeyValuePair<string, string>(">=", "GreaterThanOrEqual"),
    new KeyValuePair<string, string>("<=", "LessThanOrEqual"),
    new KeyValuePair<string, string>(">>", "GreaterThan"),
    new KeyValuePair<string, string>("<<", "LessThan"),
    new KeyValuePair<string, string>("%=", "Contains"),
    new KeyValuePair<string, string>("%>", "In")
  });

  public static Expression<Func<T, bool>> Parse<T>(string strFilter, bool caseSensitive = true)
    where T : class
  {
    // Initializing the filter with default TRUE
    var predicate = PredicateBuilder.New<T>(true);

    // Abort if string does not match pattern
    if (strFilter == null || !strFilter.StartsWith(_startPattern) || !strFilter.EndsWith(_endPattern))
      return predicate;

    // Transforming the string into an array of filters
    var filters = strFilter.Substring(_startPattern.Length, strFilter.Length - (_startPattern + _endPattern).Length)
      .Split(_separatorPattern);

    foreach (var filter in filters)
    {
      string[] keyValue;
      const int KEY = 0;
      const int VALUE = 1;

      var propertyName = "";
      var propertyValue = "";
      string method;

      var operation = _possibleOperations.FirstOrDefault(t => filter.Contains(t.Key));

      // operation not found
      if (operation.Key == null) continue;

      // setting the variables according to the operation
      keyValue = filter.Split(operation.Key);
      method = operation.Value;

      // "keyValue" is an array where the first position represents the property name
      // and the last position represents the value to be filtered in the property
      propertyName = keyValue[KEY];
      propertyValue = keyValue[VALUE];

      var hasOrStatement = propertyName.Contains(_propertySeparatorPattern);

      Expression<Func<T, bool>> lambda = t => false;

      if (!hasOrStatement)
      {
        lambda = ConvertToLambda<T>(propertyName, propertyValue, method, caseSensitive);
      }
      else
      {
        var properties = propertyName.Split(_propertySeparatorPattern);

        foreach (var property in properties)
          lambda = lambda.Or(ConvertToLambda<T>(property.FirstCharToUpper(), propertyValue, method, caseSensitive));
      }

      // Add clause to filter
      predicate.And(lambda);
    }

    return predicate;
  }

  public static Expression<Func<T, bool>> ConvertToLambda<T>(string propertyName, string propertyValue, string method,
    bool caseSentitive = true)
  {
    var parameter = Expression.Parameter(typeof(T), "t");
    var property = GetProperty(parameter, propertyName);
    var value = Parse(propertyValue, property.Type, method.Equals("In"));
    var filterValue = !method.Equals("In") ? Expression.Constant(value, property.Type) : Expression.Constant(value);
    Expression comparation = null;

    Expression<Func<T, bool>> lambdaIn = null;

    // Performs the comparison according to the operation
    switch (method)
    {
      case "Equal":
        comparation = Expression.Equal(property, filterValue);

        if (!caseSentitive && property.Type == typeof(string))
        {
          comparation = Expression.Equal(
            Expression.Call(property, "ToLower", null),
            Expression.Call(filterValue, "ToLower", null)
          );
        }

        break;
      case "GreaterThanOrEqual":
        comparation = Expression.GreaterThanOrEqual(property, filterValue);
        break;
      case "LessThanOrEqual":
        comparation = Expression.LessThanOrEqual(property, filterValue);
        break;
      case "GreaterThan":
        comparation = Expression.GreaterThan(property, filterValue);
        break;
      case "LessThan":
        comparation = Expression.LessThan(property, filterValue);
        break;
      case "Contains":
        comparation = Expression.Call(property, "Contains", null, filterValue);

        if (!caseSentitive && property.Type == typeof(string))
        {
          comparation = Expression.Call(
            Expression.Call(property, "ToLower", null),
            "Contains",
            null,
            Expression.Call(filterValue, "ToLower", null)
          );
        }

        break;
      case "In":
        lambdaIn = t => false;

        foreach (var item in value)
        {
          filterValue = Expression.Constant(item, property.Type);

          if (!caseSentitive && property.Type == typeof(string))
          {
            comparation = Expression.Equal(
              Expression.Call(property, "ToLower", null),
              Expression.Call(filterValue, "ToLower", null)
            );
          }
          else
          {
            comparation = Expression.Equal(property, filterValue);
          }

          lambdaIn = lambdaIn.Or(Expression.Lambda<Func<T, bool>>(comparation, parameter));
        }

        break;
    }

    // Create the lambda expression
    var lambda = lambdaIn ?? Expression.Lambda<Func<T, bool>>(comparation, parameter);

    return lambda;
  }

  public static T Convert<T>(string strFilter)
  {
    // Creating a new instance of the Class Type
    var convertedClass = (T)Activator.CreateInstance(typeof(T), Array.Empty<object>());

    // Abort if string does not match pattern
    if (strFilter == null || !strFilter.StartsWith(_startPattern) || !strFilter.EndsWith(_endPattern))
      return convertedClass;

    // Transforming the string into an array of filters
    var filters = strFilter.Substring(_startPattern.Length, strFilter.Length - (_startPattern + _endPattern).Length)
      .Split(_separatorPattern);

    foreach (var filter in filters)
    {
      string[] keyValue;
      const int KEY = 0;
      const int VALUE = 1;

      string propertyName;
      string propertyValue;

      // setting the variables according to the operation
      if (filter.Contains("=="))
        keyValue = filter.Split("==");
      else
        continue;

      var hasOrStatement = keyValue[KEY].Contains(_propertySeparatorPattern);
      if (hasOrStatement) continue;

      // "keyValue" is an array where the first position represents the property name
      // and the last position represents the value to be filtered in the property
      propertyName = keyValue[KEY].FirstCharToUpper();
      propertyValue = keyValue[VALUE];

      var property = typeof(T).GetProperty(propertyName);
      var value = Parse(propertyValue, property.PropertyType, false);

      // assigning property value inside class instance
      property.SetValue(convertedClass, value);
    }

    return convertedClass;
  }

  private static bool In(this Type source, params Type[] comparisons)
  {
    return comparisons.Contains(source);
  }

  private static bool IsStruct<T>(this Type source)
    where T : struct
  {
    return source.In(typeof(T), typeof(T?));
  }

  private static bool IsNullableEnum(this Type source)
  {
    var underlyingType = Nullable.GetUnderlyingType(source);
    return underlyingType?.IsEnum ?? false;
  }

  private static dynamic ParseInvariantCulture<T>(string value, bool isArray,
    Func<string, IFormatProvider, T> converter)
  {
    if (!isArray) return converter(value.Trim(), CultureInfo.InvariantCulture);
    return value.Trim()
      .Split(_inSeparatorPattern)
      .Select(t => converter(t.Trim(), CultureInfo.InvariantCulture))
      .ToArray();
  }

  private static dynamic ParseEnum(string value, bool isArray, Type enumType)
  {
    if (!isArray) return Enum.Parse(enumType, value.Trim());
    return value.Split(_inSeparatorPattern).Select(v => Enum.Parse(enumType, v.Trim())).ToArray();
  }

  private static dynamic ParseStruct(string value, Type type, bool isArray)
  {
    if (type.IsStruct<DateTime>())
      return ParseInvariantCulture(value, isArray, DateTime.Parse);

    if (type.IsStruct<bool>())
      return Parse(value, isArray, bool.Parse);

    if (type.IsStruct<Guid>())
      return Parse(value, isArray, Guid.Parse);

    if (type.IsStruct<int>())
      return ParseInvariantCulture(value, isArray, int.Parse);

    if (type.IsStruct<uint>())
      return ParseInvariantCulture(value, isArray, uint.Parse);

    if (type.IsStruct<long>())
      return ParseInvariantCulture(value, isArray, long.Parse);

    if (type.IsStruct<ulong>())
      return ParseInvariantCulture(value, isArray, ulong.Parse);

    if (type.IsStruct<short>())
      return ParseInvariantCulture(value, isArray, short.Parse);

    if (type.IsStruct<ushort>())
      return ParseInvariantCulture(value, isArray, ushort.Parse);

    if (type.IsStruct<float>())
      return ParseInvariantCulture(value, isArray, float.Parse);

    if (type.IsStruct<double>())
      return ParseInvariantCulture(value, isArray, double.Parse);

    if (type.IsStruct<decimal>())
      return ParseInvariantCulture(value, isArray, decimal.Parse);

    if (type.IsStruct<byte>())
      return ParseInvariantCulture(value, isArray, byte.Parse);

    if (type.IsStruct<sbyte>())
      return ParseInvariantCulture(value, isArray, sbyte.Parse);

    if (type.IsStruct<char>())
      return Parse(value, isArray, char.Parse);

    return null;
  }

  private static dynamic Parse<T>(string value, bool isArray, Converter<string, T> converter)
  {
    if (!isArray) return converter(value.Trim());
    return value.Trim().Split(_inSeparatorPattern).Select(t => converter(t.Trim())).ToArray();
  }

  // function used for converting string to given type
  private static dynamic Parse(string value, Type type, bool isArray)
  {
    if (type.Equals(typeof(string)) && isArray)
      return value.Split(_inSeparatorPattern).Select(t => t.Trim());

    if (type.Equals(typeof(string)))
      return value;

    if (type.IsEnum)
      return ParseEnum(value, isArray, type);

    if (type.IsNullableEnum())
      return ParseEnum(value, isArray, Nullable.GetUnderlyingType(type));

    return ParseStruct(value, type, isArray);
  }

  private static Expression GetProperty(ParameterExpression parameter, string propertyName)
  {
    var nodes = propertyName.Split('.');
    Expression body = parameter;

    for (var i = 0; i < nodes.Length; i++)
      body = Expression.PropertyOrField(body, nodes[i]);

    return body;
  }

  public static Expression<Func<TSource, TTarget>> BuildSelector<TSource, TTarget>(string members)
  {
    return BuildSelector<TSource, TTarget>(members.Split(',').Select(m => m.Trim()));
  }

  public static Expression<Func<TSource, TTarget>> BuildSelector<TSource, TTarget>(IEnumerable<string> members)
  {
    var parameter = Expression.Parameter(typeof(TSource), "e");
    var body = NewObject(typeof(TTarget), parameter, members.Select(m => m.Split('.')));
    return Expression.Lambda<Func<TSource, TTarget>>(body, parameter);
  }

  private static Expression NewObject(Type targetType, Expression source, IEnumerable<string[]> memberPaths,
    int depth = 0)
  {
    var bindings = new List<MemberBinding>();
    var target = Expression.Constant(null, targetType);
    foreach (var memberGroup in memberPaths.GroupBy(path => path[depth]))
    {
      var memberName = memberGroup.Key;
      var targetMember = Expression.PropertyOrField(target, memberName);
      var sourceMember = Expression.PropertyOrField(source, memberName);
      var childMembers = memberGroup.Where(path => depth + 1 < path.Length);
      var targetValue = !childMembers.Any()
        ? sourceMember
        : NewObject(targetMember.Type, sourceMember, childMembers, depth + 1);
      bindings.Add(Expression.Bind(targetMember.Member, targetValue));
    }

    return Expression.MemberInit(Expression.New(targetType), bindings);
  }
}
