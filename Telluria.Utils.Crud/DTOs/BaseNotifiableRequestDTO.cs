using System.Collections.Generic;
using Telluria.Utils.Crud.DTOs.Validation;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.DTOs
{
  public abstract class BaseNotifiableRequestDTO<TEntity> : Notifiable, IRequestDTO<TEntity>
  where TEntity : BaseEntity
  {
    IEnumerable<FluentValidation.Results.ValidationFailure> IRequestDTO<TEntity>.Notifications => Notifications;

    public new bool IsValid
    {
      get
      {
        ClearNotifications();
        Validate();
        return base.IsValid;
      }
    }

    public abstract void Validate();
  }
}
