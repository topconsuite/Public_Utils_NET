using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Telluria.Utils.Crud.Helpers;

namespace Telluria.Utils.Crud.Controllers
{
  public class PatchBody<TModel, TDto> : Dictionary<string, object>
    where TModel : class
    where TDto : class
  {
    public void ApplyTo(TModel model)
    {
      var errors = new List<string>();
      var properties = typeof(TDto).GetProperties();
      var propertyNames = properties.Select(t => t.Name.ToCamelCase());
      var invalidProperties = Keys.Where(key => !propertyNames.Contains(key.ToCamelCase()));

      if (invalidProperties.Any())
        errors.AddRange(invalidProperties.Select(key => $"invalid property \"{key}\""));

      if (errors.Count > 0)
        throw new Exception($"(Errors: {string.Join("; ", errors)};)");

      var stringObject = JsonSerializer.Serialize(this);
      Newtonsoft.Json.JsonConvert.PopulateObject(stringObject, model);
    }
  }
}
