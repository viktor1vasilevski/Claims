using Claims.Api.Extensions;
using Claims.Api.Middlewares;
using Claims.Application.Extensions;
using Claims.Infrastructure.Context;
using Claims.Infrastructure.Extensions;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerDocumentation();

string sqlConnectionString, mongoConnectionString;

#if DEBUG
if (builder.Configuration.GetValue<bool>("UseTestContainers"))
{
    (sqlConnectionString, mongoConnectionString) = await TestContainersExtensions.StartContainersAsync();
}
else
{
    sqlConnectionString = builder.Configuration.GetConnectionString("SqlServer")!;
    mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb")!;
}
#else
sqlConnectionString = builder.Configuration.GetConnectionString("SqlServer")!;
mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb")!;
#endif

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

builder.Services.AddHealthChecks()
    .AddSqlServer(sqlConnectionString, name: "sql-server")
    .AddMongoDb(_ => new MongoClient(mongoConnectionString), name: "mongodb");

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
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}

app.Run();

public partial class Program { }