using Peyghoom.Core.Results;

namespace Peyghoom.Services.CacheService;

public interface ICacheService
{
    public Result CachePhoneNumberOtp(long phoneNumber, string otp);
}