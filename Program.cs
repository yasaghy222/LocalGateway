using System.Text;
using ApiGateway.Common;
using ApiGateway.Common.Ocelot;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OTPService.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("Ocelot.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

string? defKey = builder.Configuration["Jwt:SecretKey"];
string? secretKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? defKey;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

// string? secretKey = builder.Configuration.GetValue<string>("Jwt:SecretKey");
if (secretKey != null)
{
    var configuration = OcelotConfiguration.GetInstance(secretKey);
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
