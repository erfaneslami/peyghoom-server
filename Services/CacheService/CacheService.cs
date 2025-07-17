using Peyghoom.Core.Results;
using Peyghoom.Services.CacheService.Models;

namespace Peyghoom.Services.CacheService;

public class CacheService: ICacheService
{
    private readonly Dictionary<long, OtpCacheInfo> _otpCache = new();
    public Result CachePhoneNumberOtp(long phoneNumber, string otp)
    {
        _otpCache.TryGetValue(phoneNumber, out var otpInfo);
        if (otpInfo != null)
        {
            return Result.Failure(Error.Conflict("Opt already exist for your phone number"));
        }
        
        _otpCache.Add(phoneNumber,new OtpCacheInfo()
        {
           Otp = otp,
           PhoneNumber = phoneNumber,
           CreateAt = new DateTime()
        });
        
        return Result.Success();
    }

    public Result<OtpCacheInfo> GetOtpInfo(long phoneNumber)
    {
        _otpCache.TryGetValue(phoneNumber, out var otpInfo);
        if (otpInfo is null) return Result.Failure<OtpCacheInfo>(Error.NotFound());
        return otpInfo;
    }
}