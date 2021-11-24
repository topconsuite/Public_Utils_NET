using Flunt.Notifications;
using System.Collections.Generic;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.DTOs
{
    public interface IRequestDTO<TEntity> where TEntity : BaseEntity
    {
        void Validate();
        IEnumerable<Notification> Notifications { get; }
        bool IsValid { get; }
    }
}
