using Microsoft.EntityFrameworkCore;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Repositories;

namespace Telluria.Utils.Crud.Tests;

public class CustomEntity : BaseEntity { }
public interface ICustomRepository : IBaseCrudRepository<CustomEntity> { }
public class CustomRepository : BaseCrudRepository<CustomEntity>, ICustomRepository
{
  public CustomRepository(DbContext context) : base(context)
  {
  }
}
