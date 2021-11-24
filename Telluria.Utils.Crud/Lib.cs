using Microsoft.Extensions.DependencyInjection;
using Telluria.Utils.Crud.Handlers;
using Telluria.Utils.Crud.Repositories;

namespace Telluria.Utils.Crud
{
    public static class Lib
    {
        public static IServiceCollection AddCrud(this IServiceCollection services)
        {
            return services
                .AddScoped(typeof(IBaseCrudRepository<>), typeof(BaseCrudRepository<>))
                .AddScoped(typeof(IBaseCrudCommandHandler<>), typeof(BaseCrudCommandHandler<>));
        }
    }
}
