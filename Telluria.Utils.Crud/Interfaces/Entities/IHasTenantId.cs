using System;

namespace Telluria.Utils.Crud.Interfaces.Entities
{
  public interface IHasTenantId
  {
    public Guid TenantId { get; set; }
  }
}
