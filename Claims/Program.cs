using Claims.Api.Extensions;
using Claims.Api.Middlewares;
using Claims.Application.Extensions;
using Claims.Application.Validations.Claims;
using Claims.Infrastructure.Context;
using Claims.Infrastructure.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerDocumentation();

var sqlConnectionString = builder.Configuration.GetConnectionString("SqlServer")!;
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb")!;

builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContexts(sqlConnectionString, mongoConnectionString, builder.Configuration["MongoDb:DatabaseName"]!);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddValidatorsFromAssemblyContaining<CreateClaimRequestValidator>(ServiceLifetime.Transient);
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks()
    .AddSqlServer(sqlConnectionString, name: "sql-server")
    .AddMongoDb(_ => new MongoClient(mongoConnectionString), name: "mongodb");

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing") || app.Environment.IsEnvironment("Staging"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}

app.Run();

public partial class Program { }