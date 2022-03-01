using System.Collections.Generic;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.DTOs
{
  public interface IRequestDTO<TEntity> where TEntity : BaseEntity
  {
    void Validate();
    List<FluentValidation.Results.ValidationFailure> Notifications { get; }
    bool IsValid { get; }
  }
}
