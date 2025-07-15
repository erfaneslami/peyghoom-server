namespace Peyghoom.Services.CacheService;

public interface ICacheService
{
    public void CachePhoneNumberOtp(long phoneNumber, string otp);
}