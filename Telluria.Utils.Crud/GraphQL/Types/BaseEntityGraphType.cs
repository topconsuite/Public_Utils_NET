using GraphQL.Types;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.GraphQL.Types
{
  public class BaseEntityGraphType<TEntity> : ObjectGraphType<TEntity>
    where TEntity : BaseEntity
  {
    public BaseEntityGraphType()
    {
      var entityName = typeof(TEntity).Name;
      Field(x => x.Id, nullable: false).Description($"The id of the {entityName}");
      Field(x => x.Deleted, nullable: false).Description($"The deleted status of the {entityName}");
      Field(x => x.CreatedAt, nullable: false).Description($"The creation date of the {entityName}");
      Field(x => x.UpdatedAt, nullable: true).Description($"The update date of the {entityName}");
      Field(x => x.DeletedAt, nullable: true).Description($"The deletion date of the {entityName}");
    }
  }
}
