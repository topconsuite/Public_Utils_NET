using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Controllers;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.GraphQL;
using Telluria.Utils.Crud.GraphQL.Types;
using Telluria.Utils.Crud.Handlers;
using Telluria.Utils.Crud.Mapping;
using Telluria.Utils.Crud.Repositories;
using Telluria.Utils.Crud.Services;
using Telluria.Utils.Crud.Validation;

namespace Telluria.Utils.Crud.Sample;
/*
 * MIGRATIONS COMMANDS:
 * -> dotnet ef migrations add InitialCreate
 * -> dotnet ef database update
 */

// Domain.Enums
public enum EProductStockType
{
  OWN,
  FROM_THIRD_PARTY,
  IN_THIRD_PARTY
}

// Domain.Entities
public class Product : BaseEntity
{
  public string Code { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public decimal Price { get; set; }
  public EProductStockType? StockType { get; set; }
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

    RuleSet("DELETE", () => { RuleFor(x => x.Id).NotEmpty(); });
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
  public ProductRepository(IServiceProvider serviceProvider, DbContext context) : base(serviceProvider, context)
  {
  }
}

// Domain.Interfaces.Handlers
public interface IProductCommandHandler : IBaseCrudCommandHandler<Product, ProductValidator, IProductRepository>
{
}

// Domain.Handlers
public class ProductCommandHandler : BaseCrudCommandHandler<Product, ProductValidator, IProductRepository>,
  IProductCommandHandler
{
  public ProductCommandHandler(ITransactionService transactionService, IProductRepository repository) : base(transactionService, repository)
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
public class
  ProductsController : BaseCrudController<Product, ProductValidator, IProductRepository, IProductCommandHandler>
{
  public ProductsController(ITenantService tenantService) : base(tenantService)
  {
  }
}

public class ProductType : BaseEntityGraphType<Product>
{
  public ProductType()
  {
    Field(x => x.Code);
    Field(x => x.Name);
    Field(x => x.Price);
    Field(x => x.StockType, true);
  }
}

public class ProductCreateInputType : BaseCreateInputType<Product>
{
  public ProductCreateInputType()
  {
    Field(x => x.Code);
    Field(x => x.Name);
    Field(x => x.Price);
    Field(x => x.StockType, true);
  }
}

public class ProductUpdateInputType : BaseUpdateInputType<Product>
{
  public ProductUpdateInputType()
  {
    Field(x => x.Code, true);
    Field(x => x.Name, true);
    Field(x => x.Price, true);
    Field(x => x.StockType, true);
  }
}

public class ProductMutation : BaseEntityMutation<Product, ProductType, ProductValidator, IProductRepository,
  IProductCommandHandler>
{
  public ProductMutation()
  {
    AddBaseMutationCreate<ProductCreateInputType>();
    AddBaseMutationUpdate<ProductUpdateInputType>();
    AddBaseMutationDelete();
  }
}

public class ProductQuery : BaseEntityQuery<Product, ProductType, ProductValidator, IProductRepository,
  IProductCommandHandler>
{
  public ProductQuery()
  {
    AddBaseQueryGetById();
    AddBaseQueryGetAll();
    AddBaseQuerySortedGetAll();
  }
}
