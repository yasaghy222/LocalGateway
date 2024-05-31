using System.Text;
using ApiGateway.Common.Ocelot;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging.Console;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

internal class Program
{
    private static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(i => i.ColorBehavior = LoggerColorBehavior.Disabled);
        });
        ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

        builder.Configuration
            .SetBasePath(builder.Environment.ContentRootPath)
             .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{builder.Environment}.json", true, true)
            .AddOcelot("Common/Ocelot/Routes", builder.Environment, MergeOcelotJson.ToMemory) // happy path
            .AddEnvironmentVariables();

        string? defKey = builder.Configuration["Jwt:SecretKey"];
        string? secretKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? defKey;

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    // ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["Public"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });



        builder.Services.AddOcelot();

        WebApplication app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseRouting();
        if (secretKey != null)
        {
            OcelotPipelineConfiguration configuration = OcelotConfiguration.GetInstance(logger);
            await app.UseOcelot(configuration);
        }
        else
        {
            throw new Exception("SecretKey is null");
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}