using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using TimeCraft.Api.Extensions;
using TimeCraft.Infrastructure.Configurations;
using TimeCraft.Infrastructure.Persistence.Data;
using TimeCraft.Infrastructure.Persistence.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.RegisterServices();

services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

services.AddScoped<IUnitOfWork, UnitOfWork>();

#region [Serilog]
var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
#endregion

#region [Mapper]
var mapperConfiguration = new MapperConfiguration(
    mc => mc.AddProfile(new AutoMapperConfigurations()));

IMapper mapper = mapperConfiguration.CreateMapper();
services.AddSingleton(mapper);
#endregion  

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var allServices = scope.ServiceProvider;

    var context = allServices.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.DocExpansion(DocExpansion.None));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
