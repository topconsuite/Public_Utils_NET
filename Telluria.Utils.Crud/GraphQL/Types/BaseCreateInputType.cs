using GraphQL.Types;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.GraphQL.Types
{
  public class BaseCreateInputType<TEntity> : InputObjectGraphType<TEntity>
    where TEntity : BaseEntity
  {
    public BaseCreateInputType()
    {
      var entityName = typeof(TEntity).Name;

      Name = $"{entityName}CreateInputType";
      Description = $"{entityName} Create Input";
    }
  }
}
