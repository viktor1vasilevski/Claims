using Claims.Api.Extensions;
using Claims.Application.Extensions;
using Claims.Infrastructure.Context;
using Claims.Infrastructure.Extensions;
using Claims.Middleware;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerDocumentation();

var (sqlConnectionString, mongoConnectionString) = await TestContainersExtensions.StartContainersAsync();

builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContexts(sqlConnectionString, mongoConnectionString, builder.Configuration["MongoDb:DatabaseName"]!);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}

app.Run();

public partial class Program { }