## Telluria.Utils.Crud

Generic library for easy creation of CRUD rest APIs with dotnet and EntityFrameworkCore.

<hr/>

### Dependencies

- .NET 6.0 [(go to site)](https://dotnet.microsoft.com/download).
- AutoMapper (>= 10.1.1) [(go to package)](https://www.nuget.org/packages/AutoMapper).
- Flunt (>= 2.0.5) [(go to package)](https://www.nuget.org/packages/Flunt).
- LinqKit.Core (>= 1.1.27) [(go to package)](https://www.nuget.org/packages/LinqKit.Core).
- Microsoft.AspNetCore.Mvc.Core (>=
  2.2.5) [(go to package)](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Core).
- Microsoft.EntityFrameworkCore.Relational (>=
  6.0.0) [(go to package)](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Relational).

<hr/>

### Instalation:

This package is available through Nuget Packages: https://www.nuget.org/packages/Telluria.Utils.Crud

**Nuget**

```
Install-Package Telluria.Utils.Crud
```

**.NET CLI**

```
dotnet add package Telluria.Utils.Crud
```

<hr/>

## How to use:

### 1. Creating Entity Models

Extend your entities from the "**BaseEntity**" class located in namespace "**Telluria.Utils.Crud.Entities**":

```csharp
using System;
using System.Collections.Generic;

// Import Library
using Telluria.Utils.Crud.Entities;

// Sample Customer Entity (simple)
public class Customer : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    ...
}

// Sample enum Type
public enum EProductType
{
    FEEDSTOCK = 1,
    FINAL_PRODUCT,
    ...
}

// Sample Product Entity (with enum type property)
public class Product : BaseEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public EProductType Type { get; set; }
    ...
}

// Sample Order Entity (with relationships)
public class Order : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    public List<OrderItem> Items { get; set; }
    ...
}

// Sample OrderItem Entity (with relationships)
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    ...
}
```

### 2. Mapping Entities

Extend your mapping classes from the "**BaseEntityMap<T>**" class located in namespace "**Telluria.Utils.Crud.Mapping
**":

```csharp
// Import Library
using Telluria.Utils.Crud.Mapping;

//  Simple mapping
public class CustomerMap : BaseEntityMap<Customer> {}

// Mapping Entity wuth enum type property
public class ProductMap : BaseEntityMap<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        // use EnumConverter<T> to map the enum property
        builder.Property(t => t.Type)
            .HasConversion(EnumConverter<EProductType>());
    }
}

// Mapping multiple relationships
public class OrderMap : BaseEntityMap<Order>
{
    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        base.Configure(builder);

        // simple relationship
        builder.HasOne(t => t.Customer)
            .WithMany()
            .HasForeignKey(t => t.CustomerId)
            .IsRequired();

        // list relationship
        builder.HasMany(t => t.Items)
            .WithOne()
            .HasForeignKey(t => t.OrderId)
            .IsRequired();
    }
}

// Mapping simple relationship
public class OrderItemMap : BaseEntityMap<OrderItem>
{
    public override void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        base.Configure(builder);

        // simple relationship
        builder.HasOne(t => t.Product)
            .WithMany()
            .HasForeignKey(t => t.ProductId)
            .IsRequired();
    }
}
```

### 3. Creating an AppDbContext

Extend your AppDbContext classes from the "**DbContext**" class located in namespace "**Microsoft.EntityFrameworkCore
**", override "**OnModelCreating**" method and add your Mapping classes to the modelBuilder:

```csharp
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    // In this sample we are using Sqlite, but you can use any database you want
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("DataSource=app.db;Cache=Shared");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerMap());
        modelBuilder.ApplyConfiguration(new ProductMap());
        modelBuilder.ApplyConfiguration(new OrderMap());
        modelBuilder.ApplyConfiguration(new OrderItemMap());
        ...
    }
}
```

### 4. Creating the Controllers

Extend your Controllers classes from the "**BaseCrudController**" class located in namespace "*
*Telluria.Utils.Crud.Controllers**":

```csharp
// Import Library
using Telluria.Utils.Crud.Controllers;

// Customers CRUD controller sample
public class CustomersController : BaseCrudController<Customer> {}

//  Products CRUD controller sample
public class ProductsController : BaseCrudController<Product> {}

//  Orders CRUD controller sample
public class OrdersController : BaseCrudController<Order> {}

//  OrderItems CRUD controller sample
public class OrderItemsController : BaseCrudController<OrderItem> {}
```

### 5. Configuring services

In your "**Startup.cs**" file, import namespace "**Telluria.Utils.Crud**" and in "**ConfigureServices**" method add the
following:

```csharp
/* Startup.cs */

// Import Library
using Telluria.Utils.Crud;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        ...

        /* --- ADD THIS 2 LINES --- */
        services.AddDbContext<AppDbContext>();
        services.AddScoped<DbContext, AppDbContext>();
        /* ---------------------- */

        services.AddControllers();
        ...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ...
    }
}
```

### Ready to go!

Note: Before running the project, remember to create the database using migrations (or manually, if you prefer)

<hr/>

## Using DTOs

This package allows you to implement "**RequestDTO**" templates for the **Create** and **Update** CRUD operations, along
with a "**ResponseDTO**" for all returns:

### 1. Implementing "**ResponseDTO**"

Extend your ResponseDTO class from the "**IResponseDTO<Customer><T>**" interface located in namespace "*
*Telluria.Utils.Crud.DTOs**":

```csharp
// Import Library
using Telluria.Utils.Crud.DTOs;

public class CustomerResponseDTO : IResponseDTO<Customer>
{
    // declare properties that you want for the DTO:
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    ...
}
```

### 2. Entering the DTO types in the controller definition

Same as previous Controller definition, but with DTO type parameters:

```csharp
// Import Library
using Telluria.Utils.Crud.Controllers;

// Pay attention to the order of parameters
public class CustomersController
    : BaseCrudController<Customer, CustomerCreateRequestDTO, CustomerUpdateRequestDTO, CustomerResponseDTO>
{
}
```

### Once again, you are ready to go!

<hr/>

## Querying and Pagination

The API generated by this library has many features available like: **Querying**, **Pagination** and **Nested Includes**
for child Entities/Lists:

### Pagination samples

```
    GET /customers?page=2&perPage=20
    GET /customers?page=1&perPage=35
```

### Querying samples

- The query has to start with "**$(**" and ends with "**)**"
- The clauses must be separated by "**;**"
- Operators:
  - "**==**" (Equal)
  - "**>=**" (GreaterThanOrEqual)
  - "**<=**" (LessThanOrEqual)
  - "**>>**" (GreaterThan)
  - "**<<**" (LessThan)
  - "**%=**" (Contains)
  - "**%>**" (In) (for this option, values must be separated by "**|**")

```
    GET /customers?where=$(email==jhondoe@gmail.com)
    GET /customers?where=$(email%=jhon;name%=jhon)
    GET /products?where=$(price>=10;type==FEEDSTOCK)
    GET /products?where=$(price<<10)
```

### Nested Includes

You can use this if there are foreign key constraints for the related items:

```
    GET /orders?include=items
    GET /orders?include=items.product
    GET /orders?include=customer
    GET /orders?include=items.product&include=customer
```

### Sorting

- The sort has to start with "**$(**" and ends with "**)**"
- The clauses must be separated by "**;**"
- Indicators
  - "**==**" (Set the order direction)

```
    GET /orders?sort=$(createdAt==desc;name==asc)
```

### All previous itens can be used together

```
    GET /orders?page=1&perPage=35&include=items.product&include=customer&where=$(customerId==<SOME_GUID>)
```
