using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using TimeCraft.Api.Extensions;
using TimeCraft.Core.Helpers;
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

builder.Configuration.AddEnvironmentVariables();
services.AddSingleton(builder.Configuration);

services.Configure<JWTConfiguration>(builder.Configuration.GetSection("JWTConfiguration"));

#region JWT 
services.AddDefaultIdentity<IdentityUser>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<DataContext>();

// Add JWT authentication
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["JWTConfiguration:Secret"]);
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = "http://localhost:37997",
        ValidAudience = "http://localhost:37997",
        RequireExpirationTime = false,
    };
});
#endregion

services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });


    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference    
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


#region [Serilog]
//var logger = new LoggerConfiguration()
//        .ReadFrom.Configuration(builder.Configuration)
//        .Enrich.FromLogContext()
//        .CreateLogger();

//builder.Logging.ClearProviders();
//builder.Logging.AddSerilog(logger);
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

app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
