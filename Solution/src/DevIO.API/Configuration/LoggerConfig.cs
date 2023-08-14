using DevIO.API.Extensions;
using Elmah.Io.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.API.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLogginConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddElmahIo(o =>
            {
                o.ApiKey = "46229af0167248e786db911e8874a854";
                o.LogId = new Guid("e89ba450-93a4-4e02-8b34-46dc6daff8a8");
            });

            services.AddHealthChecks()
                .AddElmahIoPublisher(options =>
                {
                    options.ApiKey = "46229af0167248e786db911e8874a854";
                    options.LogId = new Guid("e89ba450-93a4-4e02-8b34-46dc6daff8a8");
                    options.HeartbeatId = "API_FORNECEDORES";
                })
                .AddCheck(name: "Produtos", new SqlServerHealthChecks(configuration.GetConnectionString("DefaultConnection")))
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"));

            services.AddHealthChecksUI()
                .AddSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));

            return services;
        }
        
        public static IApplicationBuilder UseLogginConfiguration(this IApplicationBuilder app)
        {
            app.UseElmahIo();

            return app;
        }
    }
}
