using GraphQL.Types;

namespace Telluria.Utils.Crud.Sample
{
  public class GraphQLMainSchema : Schema
  {
  }
  //public class GraphQLMainSchema : Schema
  //{
  //  public GraphQLMainSchema(IServiceProvider provider) : base(provider)
  //  {
  //    Query = provider.GetRequiredService<RootQuery>();
  //    Mutation = provider.GetRequiredService<RootMutation>();
  //  }
  //}

  //public class RootMutation : ObjectGraphType
  //{
  //  public RootMutation()
  //  {
  //    Name = "Mutations";
  //    Description = "Mutations for the Topcon Customer GraphQL API";

  //    Field<ProductMutation>("Product", resolve: context => new ProductMutation());
  //  }
  //}

  //public class RootQuery : ObjectGraphType
  //{
  //  public RootQuery()
  //  {
  //    Name = "Queries";
  //    Description = "Queries for the Topcon Customer GraphQL API";

  //    Field<ProductQuery>("Product", resolve: context => new ProductQuery());
  //  }
  //}
}
