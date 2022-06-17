using System.Collections.Generic;
using GraphQL.Types;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.GraphQL.Types
{
  public class BaseUpdateInputType<TEntity> : InputObjectGraphType<TEntity>
    where TEntity : BaseEntity
  {
    public BaseUpdateInputType()
    {
      var entityName = typeof(TEntity).Name;
      Field(x => x.Id, nullable: false).Description($"The id of the {entityName}");
    }

    public override object ParseDictionary(IDictionary<string, object> value) => value;
  }
}
