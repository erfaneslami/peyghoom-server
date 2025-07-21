using Peyghoom.Core.Results;
using Peyghoom.Entities;

namespace Peyghoom.Services.AuthService;

public interface IAuthService
{
   public Result SendOto(long phoneNumber);
   public Result<string> GenerateOtpToken(long phoneNumber);
   public Result<string> GenerateRegisterToken(long phoneNumber);
   public Result<string> GenerateAccessToken(User user);
   public Result<string> GenerateRefreshToken();
   public Result ValidateOtp(long phoneNumber, string otp);
   public Result<bool> IsUserRegistered(long phoneNumber);
}