using Peyghoom.Services.CacheService.Models;

namespace Peyghoom.Services.CacheService;

public class CacheService: ICacheService
{
    private Dictionary<long, OtpCacheInfo> OtpCache { get; set; } = new();
    public void CachePhoneNumberOtp(long phoneNumber, string otp)
    {
        OtpCache.Add(phoneNumber,new OtpCacheInfo()
        {
           Otp = otp,
           PhoneNumber = phoneNumber,
           CreateAt = new DateTime()
        });
    }
}