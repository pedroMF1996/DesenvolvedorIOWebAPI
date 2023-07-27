using Microsoft.AspNetCore.Mvc;

namespace DevIO.API.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection UseWebApiConfig(this IServiceCollection services)
        {
            services.AddControllers();
            services.ResolveDependencies();
            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            });

            services.ConfigCors();

            return services;
        }
        
        public static IApplicationBuilder UseMVCConfig(this IApplicationBuilder app)
        {

            app.UseCors("Development");
            app.UseHttpsRedirection();
            app.UseAuthorization();

            return app;
        }
    }
}
