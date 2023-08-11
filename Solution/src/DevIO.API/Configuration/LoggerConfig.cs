using Elmah.Io.Extensions.Logging;

namespace DevIO.API.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLogginConfiguration(this IServiceCollection services)
        {
            services.AddElmahIo(o =>
            {
                o.ApiKey = "46229af0167248e786db911e8874a854";
                o.LogId = new Guid("e89ba450-93a4-4e02-8b34-46dc6daff8a8");
            });

            //Elmah
            //services.AddLogging(builder =>
            //{
            //    builder.AddElmahIo(o =>
            //    {
            //        o.ApiKey = "46229af0167248e786db911e8874a854";
            //        o.LogId = new Guid("e89ba450-93a4-4e02-8b34-46dc6daff8a8");
            //    });

            //    builder.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning);
            //});

            return services;
        }
        
        public static IApplicationBuilder UseLogginConfiguration(this IApplicationBuilder app)
        {
            app.UseElmahIo();

            return app;
        }
    }
}
