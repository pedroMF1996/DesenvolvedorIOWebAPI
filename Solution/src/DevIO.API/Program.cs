using DevIO.API.Configuration;
using DevIO.API.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.ResolveDbConnections(builder.Configuration);

builder.Services.AddIdentityConfiguration(builder.Configuration);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddWebApiConfig(builder.Configuration);

builder.Services.AddSwaggerConfig();

builder.Services.ResolveDependencies();

builder.Services.AddLogginConfiguration(builder.Configuration);



var app = builder.Build(); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCors("Development");
}
else
{
    app.UseCors("Production");
    app.UseHsts();
}

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseSwaggerConfig(provider);


app.UseLogginConfiguration();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.UseMVCConfig();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/api/hc", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    endpoints.MapHealthChecksUI(options =>
    {
        options.UIPath = "/api/hc-ui";
        options.ResourcesPath = "/api/hc-ui-resources";

        options.UseRelativeApiPath = false;
        options.UseRelativeResourcesPath = false;
        options.UseRelativeWebhookPath = false;
    });

});

app.Run();
