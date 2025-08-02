using Peyghoom.Core.Results;
using Peyghoom.Services.CacheService.Models;

namespace Peyghoom.Services.CacheService;

public interface ICacheService
{
    public Result CachePhoneNumberOtp(long phoneNumber, long otp);
    public Result<OtpCacheInfo> GetOtpInfo(long phoneNumber);
}