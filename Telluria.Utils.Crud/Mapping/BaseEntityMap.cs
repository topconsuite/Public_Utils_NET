using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Mapping;

public abstract class BaseEntityMap<TEntity> : IEntityTypeConfiguration<TEntity>
  where TEntity : BaseEntity
{
  public virtual void Configure(EntityTypeBuilder<TEntity> builder)
  {
    ApplyDefaultConfiguration(builder);
  }

  private void ApplyDefaultConfiguration(EntityTypeBuilder<TEntity> builder)
  {
    builder.HasKey(t => t.Id);
    builder.HasQueryFilter(t => !t.Deleted);
  }

  protected ValueConverter<TEnum, string> EnumConverter<TEnum>()
    where TEnum : Enum
  {
    return new ValueConverter<TEnum, string>(t => t.ToString(), t => (TEnum)Enum.Parse(typeof(TEnum), t));
  }
}
