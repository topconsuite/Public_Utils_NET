using System.Text.Json.Serialization;
using GraphQL;
using GraphQL.MicrosoftDI;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using Telluria.Utils.Crud.Middlewares;
using Telluria.Utils.Crud.Sample;
using Telluria.Utils.Crud.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
  .AddJsonOptions(opts => {
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<DbContext, AppDbContext>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductCommandHandler, ProductCommandHandler>();
builder.Services.AddScoped<ITenantService, TenantService>();

// Add schema and register GraphQL
builder.Services.AddSingleton<ISchema, GraphQLMainSchema>(services =>
  new GraphQLMainSchema(new SelfActivatingServiceProvider(services)));

//GQLDI.GraphQLBuilderExtensions.AddGraphQL(builder.Services)
//  .AddServer(true)
//  .ConfigureExecution(options =>
//    options.EnableMetrics = true)
//  .AddSystemTextJson()
//  .AddDataLoader()
//  .AddGraphTypes(typeof(GraphQLMainSchema).Assembly);

builder.Services.AddGraphQL(b => b
    .ConfigureExecutionOptions(options =>
      options.EnableMetrics = true)
    .AddSystemTextJson()
    .AddDataLoader()
    .AddGraphTypes(typeof(GraphQLMainSchema).Assembly));   // serializer


var app = builder.Build();


app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();

  app.UseGraphQLGraphiQL();

  app.UseGraphQLPlayground();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<TenantResolver>();

app.MapControllers();

app.UseGraphQL("/graphql");
//app.UseGraphQL<ISchema>();

using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  db.Database.Migrate();
}

app.Run();
