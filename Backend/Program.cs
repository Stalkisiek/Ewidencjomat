global using AutoMapper;
global using Ewidencjomat.Data;
global using Ewidencjomat.Models;
using Ewidencjomat.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Ewidencjomat;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        
        //Automapper
        builder.Services.AddAutoMapper(typeof(Program).Assembly);
        
        //EntityFrameworkCore
        builder.Services.AddDbContext<DataContext>(options => 
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        );
        
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
        
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = """Standard Authorization header using Bearer scheme. Example: "bearer {token}" """,
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            c.OperationFilter<SecurityRequirementsOperationFilter>();
        });
        builder.Services.AddHttpContextAccessor();
        
        //scopes
        builder.Services.AddScoped<IAuthRepository, AuthRepository>();
        builder.Services.AddScoped<IUserServices, UserServices>();
        
        var app = builder.Build();
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}