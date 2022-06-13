using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Controllers;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Handlers;
using Telluria.Utils.Crud.Mapping;
using Telluria.Utils.Crud.Repositories;
using Telluria.Utils.Crud.Validation;

namespace Telluria.Utils.Crud.Sample
{
  // Domain.Entities
  public class Product : BaseEntity
  {
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
  }

  // Domain.Validators
  public class ProductValidator : BaseEntityValidator<Product>
  {
    public ProductValidator()
    {
      var upsertRule = () =>
      {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Price).NotEmpty().GreaterThan(0);
      };

      AddBaseRuleCreate(upsertRule);
      AddBaseRuleUpdate(upsertRule);
    }
  }

  // Data.Mapping
  public class ProductMap : BaseEntityMap<Product>
  {
  }

  // Domain.Interfaces.Repositories
  public interface IProductRepository : IBaseCrudRepository<Product>
  {
  }

  // Data.Repositories
  public class ProductRepository : BaseCrudRepository<Product>, IProductRepository
  {
    public ProductRepository(DbContext context) : base(context)
    {
    }
  }

  // Domain.Interfaces.Handlers
  public interface IProductCommandHandler : IBaseCrudCommandHandler<Product, ProductValidator, IProductRepository>
  {
  }

  // Domain.Handlers
  public class ProductCommandHandler : BaseCrudCommandHandler<Product, ProductValidator, IProductRepository>, IProductCommandHandler
  {
    public ProductCommandHandler(IProductRepository repository) : base(repository)
    {
    }

    protected override string GetSuccessMessage(EBaseCrudCommands command)
    {
      return command switch
      {
        // EBaseCrudCommands contains ALL base CRUD operations
        EBaseCrudCommands.CREATE => "Sample Create Message (use translations here)",
        EBaseCrudCommands.UPDATE => "Sample Update Message (use translations here)",
        /* (specify other operations in EBaseCrudCommands as needed) */
        _ => GetDefaultSuccessMessage(command)
      };
    }

    protected override ICommandResult HandleErrors(Exception exception)
    {
      var result = GetDefaultError(exception);

      /* (handle specific entity errors here) */

      return result;
    }
  }

  // API.Controllers
  public class ProductsController : BaseCrudController<Product, ProductValidator, IProductRepository, IProductCommandHandler>
  {
  }
}
