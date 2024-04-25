using System;

namespace Telluria.Utils.Crud.Services;

public interface ITenantService
{
  Guid TenantId { get; set; }
  bool HasRequest { get; set; }
}
