using System;
using System.Collections.Generic;

namespace Telluria.Utils.Crud.Helpers;

public static class EnumerableHelper
{
  public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
  {
    foreach (var item in enumerable) action(item);
  }
}
