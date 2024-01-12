using System;

namespace Telluria.Utils.Crud.Entities;

public class IBaseEntity
{
  public Guid TenantId { get; set; }
  public Guid Id { get; set; }
  public bool Deleted { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public DateTime? DeletedAt { get; set; }
}
