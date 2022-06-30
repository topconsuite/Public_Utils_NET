using System;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Handlers;
using Telluria.Utils.Crud.Repositories;
using Telluria.Utils.Crud.Validation;

namespace Telluria.Utils.Crud.Tests;

public class CustomEntity : BaseEntity { }

public interface ICustomRepository : IBaseCrudRepository<CustomEntity> { }

public class CustomRepository : BaseCrudRepository<CustomEntity>, ICustomRepository
{
  public CustomRepository(DbContext context) : base(context)
  {
  }
}

public class CustomValidator : BaseEntityValidator<CustomEntity>
{
  public CustomValidator()
  {
    var upsertRuleSet = () =>
    {
      RuleFor(x => x.Id).NotEmpty().WithMessage("Id Should be empty");
    };

    AddBaseRuleCreate(upsertRuleSet);
    AddBaseRuleUpdate(upsertRuleSet);
  }
}

public interface ICustomHandler : IBaseCrudCommandHandler<CustomEntity, CustomValidator, ICustomRepository> { }

public class CustomHandler : BaseCrudCommandHandler<CustomEntity, CustomValidator, ICustomRepository>, ICustomHandler
{
  public CustomHandler(ICustomRepository repository) : base(repository) { }

  protected override string GetSuccessMessage(EBaseCrudCommands command)
  {
    return GetDefaultSuccessMessage(command);
  }

  protected override ICommandResult HandleErrors(Exception exception)
  {
    return GetDefaultError(exception);
  }
}
