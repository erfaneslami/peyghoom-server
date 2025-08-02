using Microsoft.Extensions.Caching.Memory;
using Peyghoom.Core.Results;
using Peyghoom.Services.CacheService.Models;

namespace Peyghoom.Services.CacheService;

public class CacheService: ICacheService
{
    private readonly Dictionary<long, OtpCacheInfo> _otpCache = new();
    private readonly IMemoryCache _memoryCache;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Result CachePhoneNumberOtp(long phoneNumber, long otp)
    {
        _memoryCache.TryGetValue(phoneNumber, out var otpInfo);
        
        if (otpInfo != null)
        {
            return Result.Failure(Error.Conflict("Opt already exist for your phone number"));
        }
        _memoryCache.Set(phoneNumber, new OtpCacheInfo()
        {
            Otp = otp,
            PhoneNumber = phoneNumber,
            CreateAt = new DateTime()
        }, TimeSpan.FromSeconds(120));

        return Result.Success();

    }

    public Result<OtpCacheInfo> GetOtpInfo(long phoneNumber)
    {
        _memoryCache.TryGetValue<OtpCacheInfo>(phoneNumber, out var otpInfo);
        if (otpInfo is null) return Result.Failure<OtpCacheInfo>(Error.NotFound());
        return otpInfo;
    }
}