using Peyghoom.Core.Results;

namespace Peyghoom.Services.AuthService;

public interface IAuthService
{
   public Result SendOto(long phoneNumber);

   public Result<string> GenerateOtpToken(long phoneNumber);
   
   public Result<string> GenerateRegisterToken(long phoneNumber);
   public void ValidateOtp(long phoneNumber, string otp);
   public void RetrievePhoneNumber(string otpToken);
}