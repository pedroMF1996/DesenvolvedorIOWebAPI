namespace DevIO.API.Configuration
{
    public static class CorsConfig
    {
        public static IServiceCollection ConfigCors(this IServiceCollection services)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy("Development",
                    builder => builder.AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowCredentials());
            });
            return services;
        }
    }
}
