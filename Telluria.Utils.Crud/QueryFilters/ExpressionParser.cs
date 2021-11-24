using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Telluria.Utils.Crud.Helpers;

namespace Telluria.Utils.Crud.QueryFilters
{
    public static class ExpressionParser
    {
        // Definição do padrão da string
        private static readonly string startPattern = "$(";
        private static readonly string endPattern = ")";
        private static readonly char separatorPattern = ';';
        private static readonly char propertySeparatorPattern = '|';

        public static Expression<Func<T, bool>> Parse<T>(string strFilter) where T : class
        {
            // Inicializando o filtro com default TRUE
            var predicate = PredicateBuilder.New<T>(true);

            // Se a string está preenchida com o padrão
            if (strFilter != null && strFilter.StartsWith(startPattern) && strFilter.EndsWith(endPattern))
            {
                // Transformando a string em um array de filtros utilizando o separador
                var filters = strFilter.Substring(startPattern.Length, strFilter.Length - (startPattern + endPattern).Length).Split(separatorPattern);

                // percorrendo o array de filtros
                foreach (var filter in filters)
                {
                    string[] _keyValue; const int KEY = 0; const int VALUE = 2;

                    string _propertyName = "";
                    string _propertyValue = "";
                    string _method;

                    // verificando o tipo de operação e preenchendo as variáveis de acordo com a operação
                    if (filter.Contains("=="))
                    {
                        _keyValue = filter.Split(new char[] { '=', '=' });
                        _method = "Equal";
                    }
                    else if (filter.Contains(">="))
                    {
                        _keyValue = filter.Split(new char[] { '>', '=' });
                        _method = "GreaterThanOrEqual";
                    }
                    else if (filter.Contains("<="))
                    {
                        _keyValue = filter.Split(new char[] { '<', '=' });
                        _method = "LessThanOrEqual";
                    }
                    else if (filter.Contains(">>"))
                    {
                        _keyValue = filter.Split(new char[] { '>', '>' });
                        _method = "GreaterThan";
                    }
                    else if (filter.Contains("<<"))
                    {
                        _keyValue = filter.Split(new char[] { '<', '<' });
                        _method = "LessThan";
                    }
                    else if (filter.Contains("%="))
                    {
                        _keyValue = filter.Split(new char[] { '%', '=' });
                        _method = "Contains";
                    }
                    else if (filter.Contains("%>"))
                    {
                        _keyValue = filter.Split(new char[] { '%', '>' });
                        _method = "In";
                    }
                    else
                    {
                        continue;
                    }

                    // keyValue é um array onde a primeira posição representa o nome da propriedade 
                    // e a ultima posição representa o valor a ser filtrado na propriedade
                    _propertyName = _keyValue[KEY];
                    _propertyValue = _keyValue[VALUE];

                    // codigo utilizado para OR
                    var _hasOrStatement = _propertyName.Contains(propertySeparatorPattern);

                    Expression<Func<T, bool>> lambda = (t => false);

                    if (!_hasOrStatement)
                    {
                        lambda = ConvertToLambda<T>(_propertyName, _propertyValue, _method);
                    }
                    else
                    {
                        var _propertyNames = _propertyName.Split(propertySeparatorPattern);

                        foreach (var propertyName in _propertyNames)
                        {
                            lambda = lambda.Or(ConvertToLambda<T>(propertyName.FirstCharToUpper(), _propertyValue, _method));
                        }
                    }

                    // Adiciona a cláusula ao filtro
                    predicate.And(lambda);
                }
            }

            return predicate;
        }

        public static Expression<Func<T, bool>> ConvertToLambda<T>(string _propertyName, string _propertyValue, string _method)
        {
            var parameter = Expression.Parameter(typeof(T), "t");
            var property = _getProperty(parameter, _propertyName);
            var value = _parse(_propertyValue, property.Type, _method.Equals("In"));
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
                    lambdaIn = (t => false);
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
            var lambda = (lambdaIn == null) ? Expression.Lambda<Func<T, bool>>(comparation, parameter) : lambdaIn;

            return lambda;
        }

        public static T Convert<T>(string strFilter)
        {
            // Criando uma nava instancia do Tipo da classe
            T convertedClass = (T)Activator.CreateInstance(typeof(T), new object[] { });

            // Se a string está preenchida com o padrão
            if (strFilter != null && strFilter.StartsWith(startPattern) && strFilter.EndsWith(endPattern))
            {
                // Transformando a string em um array de filtros utilizando o separador
                var filters = strFilter.Substring(startPattern.Length, strFilter.Length - (startPattern + endPattern).Length).Split(separatorPattern);

                // percorrendo o array de filtros
                foreach (var filter in filters)
                {
                    string[] _keyValue; const int KEY = 0; const int VALUE = 2;

                    string _propertyName = "";
                    string _propertyValue = "";

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
                    var _hasOrStatement = _keyValue[KEY].Contains(propertySeparatorPattern);
                    if (_hasOrStatement) continue;

                    // keyValue é um array onde a primeira posição representa o nome da propriedade 
                    // e a ultima posição representa o valor a ser filtrado na propriedade
                    _propertyName = _keyValue[KEY].FirstCharToUpper();
                    _propertyValue = _keyValue[VALUE];

                    var property = typeof(T).GetProperty(_propertyName);
                    var value = _parse(_propertyValue, property.PropertyType, false);

                    // atribuindo o valor da propriedade dentro da instancia da classe
                    property.SetValue(convertedClass, value);

                }
            }

            return convertedClass;
        }

        // Função privada utilizada para conversão de string para determinado tipo
        private static dynamic _parse(string value, Type type, bool isArray)
        {
            char inSeparatorPattern = ',';

            if (type.FullName.Contains(".Int"))
            {
                if (!isArray) return int.Parse(value.Trim());
                else return Array.ConvertAll(value.Split(inSeparatorPattern).Select(t => t.Trim()).ToArray(), int.Parse);
            }
            else if (type.FullName.Contains(".UInt"))
            {
                if (!isArray) return uint.Parse(value.Trim());
                else return Array.ConvertAll(value.Split(inSeparatorPattern).Select(t => t.Trim()).ToArray(), uint.Parse);
            }
            else if (type.FullName.Contains(".DateTime"))
            {
                if (!isArray) return DateTime.Parse(value.Trim());
                else return Array.ConvertAll(value.Trim().Split(inSeparatorPattern).Select(t => t.Trim()).ToArray(), DateTime.Parse);
            }
            else if (type.FullName.Contains(".Bool"))
            {
                if (!isArray) return bool.Parse(value.Trim());
                else return Array.ConvertAll(value.Split(inSeparatorPattern).Select(t => t.Trim()).ToArray(), bool.Parse);
            }
            else if (type.FullName.Contains(".String"))
            {
                if (!isArray) return value;
                else return value.Split(inSeparatorPattern).Select(t => t.Trim());
            }
            else if (type.FullName.Contains(".Domain.Enums"))
            {
                if (!isArray)
                {
                    if (type.FullName.Contains("Nullable"))
                        return Enum.Parse(Nullable.GetUnderlyingType(type), value.Trim());
                    else return Enum.Parse(type, value.Trim());
                }
                else return value.Split(inSeparatorPattern).Select(v => Enum.Parse(type, v.Trim())).ToArray();
            }
            else if (type.FullName.Contains(".Guid"))
            {
                if (!isArray) return Guid.Parse(value.Trim());
                else return Array.ConvertAll(value.Split(inSeparatorPattern).Select(t => t.Trim()).ToArray(), Guid.Parse);
            }
            else
            {
                return null;
            }
        }

        private static Expression _getProperty(ParameterExpression parameter, string propertyName)
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

        static Expression NewObject(Type targetType, Expression source, IEnumerable<string[]> memberPaths, int depth = 0)
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
