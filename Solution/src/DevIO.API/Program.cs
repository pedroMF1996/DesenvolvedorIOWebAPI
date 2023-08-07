using DevIO.API.Configuration;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfig();

builder.Services.AddEndpointsApiExplorer();
builder.Services.UseWebApiConfig(builder.Configuration);

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

app.UseMVCConfig();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();
app.Run();
