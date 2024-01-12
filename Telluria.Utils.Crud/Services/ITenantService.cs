using System;

namespace Telluria.Utils.Crud.Services;

public interface ITenantService
{
  Guid TenantId { get; set; }
  public bool SetTenant(Guid tenant);
}
