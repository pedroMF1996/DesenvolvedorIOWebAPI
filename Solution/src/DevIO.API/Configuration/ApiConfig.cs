using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.API.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection UseWebApiConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddEndpointsApiExplorer();
            
            services.AddIdentityConfiguration(configuration);
            services.ResolveDbConnections(configuration);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.ResolveDependencies();
            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme);

                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();

                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            });

            services.ConfigCors();
            services.VersionSettings();

            return services;
        }

        public static IApplicationBuilder UseMVCConfig(this IApplicationBuilder app)
        {

            app.UseHttpsRedirection();

            return app;
        }

        private static IServiceCollection VersionSettings(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ReportApiVersions = true; 
            });

            services.AddVersionedApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'VVV";
                opt.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}
