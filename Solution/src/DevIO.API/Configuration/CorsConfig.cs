using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

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

                opt.AddPolicy("Production",
                    builder =>
                        builder
                        .WithMethods("GET")
                        .WithOrigins("http://desenvolvedor.io")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        //.WithHeaders(HeaderNames.ContentType,"x-custom-header")
                        .AllowAnyHeader());
            });
            return services;
        }
    }
}
