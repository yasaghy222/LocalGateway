using CustomResponce.Models;

namespace OTPService.Common
{
    public class CustomErrors
    {
        public static Result InvalidToken() => new()
        {
            Message = new()
            {
                Fa = "توکن وارد شده نامعتبر می باشد",
                En = "Invalid Token. Unauthorized 401 !"
            },
            StatusCode = StatusCodes.Status401Unauthorized,
            Status = false
        };
    }
}
