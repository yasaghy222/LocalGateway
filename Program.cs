using System.Text;
using ApiGateway.Common;
using ApiGateway.Common.Ocelot;
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

// string AuthenticationProviderKey = builder.Configuration.GetValue<string>("Jwt:SecretKey");
// builder.Services
//     .AddAuthentication()
//     .AddJwtBearer(AuthenticationProviderKey, options =>
//     {
//         // options.Authority = "http://localhost:5254";
//         options.TokenValidationParameters = new () {
//             ValidateLifetime = true,
//             ValidateIssuer = false,
//             ValidateAudience = false,
//             ValidateIssuerSigningKey = true,
//             // ValidIssuer = "http://localhost:5280",
//             // ValidAudience = "",
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthenticationProviderKey))
//         };
//     });


builder.Services.AddOcelot();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

string? secretKey = builder.Configuration.GetValue<string>("Jwt:SecretKey");
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
