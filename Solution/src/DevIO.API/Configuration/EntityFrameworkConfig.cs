using DevIO.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DevIO.API.Configuration
{
    public static class EntityFrameworkConfig
    {
        public static IServiceCollection ResolveDbConnections(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MeuDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}
