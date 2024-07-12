using System;

namespace Telluria.Utils.Crud.Entities
{
  public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public bool Deleted { get; set; } // For "Soft Delete"

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; } // For "Soft Delete"
    }
}
