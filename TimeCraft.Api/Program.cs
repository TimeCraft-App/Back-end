using Microsoft.EntityFrameworkCore;
using TimeCraft.Infrastructure.Persistence.Data;
using AutoMapper;
using TimeCraft.Infrastructure.Configurations;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
