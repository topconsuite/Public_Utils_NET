using System.Collections.Generic;
using Flunt.Notifications;
using Flunt.Validations;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.DTOs
{
  public abstract class BaseNotifiableRequestDTO<TEntity> : Notifiable<Notification>, IRequestDTO<TEntity> where TEntity : BaseEntity
  {
    protected Contract<IRequestDTO<TEntity>> _contract => new Contract<IRequestDTO<TEntity>>();

    IEnumerable<Notification> IRequestDTO<TEntity>.Notifications => Notifications;

    public new bool IsValid
    {
      get
      {
        Clear();
        Validate();
        return base.IsValid;
      }
    }

    public abstract void Validate();
  }
}
