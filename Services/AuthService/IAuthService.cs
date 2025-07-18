using Peyghoom.Core.Results;

namespace Peyghoom.Services.AuthService;

public interface IAuthService
{
   public Result SendOto(long phoneNumber);
   public Result<string> GenerateOtpToken(long phoneNumber);
   public Result<string> GenerateRegisterToken(long phoneNumber);
   public Result<string> GenerateAccessToken(long phoneNumber);
   public Result<string> GenerateRefreshToken(long phoneNumber);
   public Result ValidateOtp(long phoneNumber, string otp);
   public Result<bool> IsUserRegistered(long phoneNumber);
}