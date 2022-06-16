using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Telluria.Utils.Crud.Helpers;

namespace Telluria.Utils.Crud.QueryFilters
{
  public static class ExpressionParser
  {
    // Definição do padrão da string
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
      new KeyValuePair<string, string>("%>", "In"),
    });

    public static Expression<Func<T, bool>> Parse<T>(string strFilter)
      where T : class
    {
      // Inicializando o filtro com default TRUE
      var predicate = PredicateBuilder.New<T>(true);

      // Se a string não está preenchida com o padrão
      if (strFilter == null || !strFilter.StartsWith(_startPattern) || !strFilter.EndsWith(_endPattern))
        return predicate;

      // Transformando a string em um array de filtros utilizando o separador
      var filters = strFilter.Substring(_startPattern.Length, strFilter.Length - (_startPattern + _endPattern).Length).Split(_separatorPattern);

      // percorrendo o array de filtros
      foreach (var filter in filters)
      {
        string[] _keyValue;
        const int KEY = 0;
        const int VALUE = 1;

        string _propertyName = "";
        string _propertyValue = "";
        string _method;

        var operation = _possibleOperations.FirstOrDefault(t => filter.Contains(t.Key));

        // operação não encontrada
        if (operation.Key == null) continue;

        // verificando o tipo de operação e preenchendo as variáveis de acordo com a operação
        _keyValue = filter.Split(operation.Key);
        _method = operation.Value;

        // keyValue é um array onde a primeira posição representa o nome da propriedade
        // e a ultima posição representa o valor a ser filtrado na propriedade
        _propertyName = _keyValue[KEY];
        _propertyValue = _keyValue[VALUE];

        // codigo utilizado para OR
        var _hasOrStatement = _propertyName.Contains(_propertySeparatorPattern);

        Expression<Func<T, bool>> lambda = t => false;

        if (!_hasOrStatement)
        {
          lambda = ConvertToLambda<T>(_propertyName, _propertyValue, _method);
        }
        else
        {
          var _propertyNames = _propertyName.Split(_propertySeparatorPattern);

          foreach (var propertyName in _propertyNames)
          {
            lambda = lambda.Or(ConvertToLambda<T>(propertyName.FirstCharToUpper(), _propertyValue, _method));
          }
        }

        // Adiciona a cláusula ao filtro
        predicate.And(lambda);
      }

      return predicate;
    }

    public static Expression<Func<T, bool>> ConvertToLambda<T>(string _propertyName, string _propertyValue, string _method)
    {
      var parameter = Expression.Parameter(typeof(T), "t");
      var property = GetProperty(parameter, _propertyName);
      var value = Parse(_propertyValue, property.Type, _method.Equals("In"));
      var filterValue = !_method.Equals("In") ? Expression.Constant(value, property.Type) : Expression.Constant(value);
      Expression comparation = null;

      Expression<Func<T, bool>> lambdaIn = null;

      // Realiza a comparação de acordo com a operação
      switch (_method)
      {
        case "Equal":
          comparation = Expression.Equal(property, filterValue);
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
          break;
        case "In":
          lambdaIn = t => false;
          foreach (var item in value)
          {
            filterValue = Expression.Constant(item, property.Type);
            comparation = Expression.Equal(property, filterValue);
            lambdaIn = lambdaIn.Or(Expression.Lambda<Func<T, bool>>(comparation, parameter));
          }

          break;
        default:
          break;
      }

      // Cria a expressão lambda
      var lambda = lambdaIn ?? Expression.Lambda<Func<T, bool>>(comparation, parameter);

      return lambda;
    }

    public static T Convert<T>(string strFilter)
    {
      // Criando uma nava instancia do Tipo da classe
      T convertedClass = (T)Activator.CreateInstance(typeof(T), Array.Empty<object>());

      // Se a string não está preenchida com o padrão
      if (strFilter == null || !strFilter.StartsWith(_startPattern) || !strFilter.EndsWith(_endPattern))
        return convertedClass;

      // Transformando a string em um array de filtros utilizando o separador
      var filters = strFilter.Substring(_startPattern.Length, strFilter.Length - (_startPattern + _endPattern).Length).Split(_separatorPattern);

      // percorrendo o array de filtros
      foreach (var filter in filters)
      {
        string[] _keyValue;
        const int KEY = 0;
        const int VALUE = 2;

        string _propertyName;
        string _propertyValue;

        // verificando o tipo de operação e preenchendo as variáveis de acordo com a operação
        if (filter.Contains("=="))
        {
          _keyValue = filter.Split(new char[] { '=', '=' });
        }
        else
        {
          continue;
        }

        // codigo utilizado para OR
        var _hasOrStatement = _keyValue[KEY].Contains(_propertySeparatorPattern);
        if (_hasOrStatement) continue;

        // keyValue é um array onde a primeira posição representa o nome da propriedade
        // e a ultima posição representa o valor a ser filtrado na propriedade
        _propertyName = _keyValue[KEY].FirstCharToUpper();
        _propertyValue = _keyValue[VALUE];

        var property = typeof(T).GetProperty(_propertyName);
        var value = Parse(_propertyValue, property.PropertyType, false);

        // atribuindo o valor da propriedade dentro da instancia da classe
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
      Type underlyingType = Nullable.GetUnderlyingType(source);
      return underlyingType?.IsEnum ?? false;
    }

    private static dynamic ParseInvariantCulture<T>(string value, bool isArray, Func<string, IFormatProvider, T> converter)
    {
      if (!isArray) return converter(value.Trim(), CultureInfo.InvariantCulture);
      return value.Trim().Split(_inSeparatorPattern).Select(t => converter(t.Trim(), CultureInfo.InvariantCulture)).ToArray();
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

    // Função privada utilizada para conversão de string para determinado tipo
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
      Expression _body = parameter;

      for (int i = 0; i < nodes.Length; i++)
        _body = Expression.PropertyOrField(_body, nodes[i]);

      return _body;
    }

    public static Expression<Func<TSource, TTarget>> BuildSelector<TSource, TTarget>(string members) =>
        BuildSelector<TSource, TTarget>(members.Split(',').Select(m => m.Trim()));

    public static Expression<Func<TSource, TTarget>> BuildSelector<TSource, TTarget>(IEnumerable<string> members)
    {
      var parameter = Expression.Parameter(typeof(TSource), "e");
      var body = NewObject(typeof(TTarget), parameter, members.Select(m => m.Split('.')));
      return Expression.Lambda<Func<TSource, TTarget>>(body, parameter);
    }

    private static Expression NewObject(Type targetType, Expression source, IEnumerable<string[]> memberPaths, int depth = 0)
    {
      var bindings = new List<MemberBinding>();
      var target = Expression.Constant(null, targetType);
      foreach (var memberGroup in memberPaths.GroupBy(path => path[depth]))
      {
        var memberName = memberGroup.Key;
        var targetMember = Expression.PropertyOrField(target, memberName);
        var sourceMember = Expression.PropertyOrField(source, memberName);
        var childMembers = memberGroup.Where(path => depth + 1 < path.Length);
        var targetValue = !childMembers.Any() ? sourceMember :
            NewObject(targetMember.Type, sourceMember, childMembers, depth + 1);
        bindings.Add(Expression.Bind(targetMember.Member, targetValue));
      }

      return Expression.MemberInit(Expression.New(targetType), bindings);
    }
  }
}
