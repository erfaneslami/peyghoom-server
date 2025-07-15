using Peyghoom.Services.CacheService;

namespace Peyghoom.Services.AuthService;

public class AuthService: IAuthService
{
    private readonly ICacheService _cacheService;

    public AuthService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public void SendOto(long phoneNumber)
    {
        // TODO: create random 5 digit number
        // TODO: store opt with information into cache
        // TODO: send otp to user phone
        throw new NotImplementedException();
    }

    public void ValidateOtp(long phoneNumber, string otp)
    {
        throw new NotImplementedException();
    }

    public void RetrievePhoneNumber(string otpToken)
    {
        throw new NotImplementedException();
    }
}