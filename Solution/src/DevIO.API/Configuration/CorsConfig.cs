using Microsoft.Extensions.Options;

namespace DevIO.API.Configuration
{
    public static class CorsConfig
    {
        public static IServiceCollection ConfigCors(this IServiceCollection services)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy("Development",
                                builder =>
                                    builder
                                    .AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });
            return services;
        }
    }
}
