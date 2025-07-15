namespace Peyghoom.Services.AuthService;

public interface IAuthService
{
   public void SendOto(long phoneNumber);
   public void ValidateOtp(long phoneNumber, string otp);
   public void RetrievePhoneNumber(string otpToken);
}