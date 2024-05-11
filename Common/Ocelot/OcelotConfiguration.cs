using System.Security.Claims;
using Ocelot.Errors;
using Ocelot.Middleware;
using OTPService.Common;

namespace ApiGateway.Common.Ocelot
{
    public class OcelotConfiguration
    {
        public static OcelotPipelineConfiguration‍ GetInstance(string secretKey)
        {
            return new OcelotPipelineConfiguration‍
            {
                PreErrorResponderMiddleware = async (context, next) =>
                {
                    await next.Invoke();


                    // string? token = context.Request.Headers["Token"];
                    // var isValid = JWTValidator.ValidateToken(token, secretKey);
                    // if (isValid)
                    // {
                    //     await next.Invoke();
                    // }
                    // else
                    // {
                    //     var result = CustomErrors.InvalidToken();
                    //     context.Response.StatusCode = result.StatusCode;
                    //     await context.Response.WriteAsJsonAsync(result);
                    // }
                },
                AuthorizationMiddleware = async (context, next) =>
                {
                    try
                    {
                        // string? token = context.Request.Headers["Token"];

                        // if (token != null)
                        // {
                        //     string base64EncodedData = token.Split(".")[1];
                        //     // var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                        //     // var data = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                        // }
                        await next.Invoke();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            };
        }
    }
}