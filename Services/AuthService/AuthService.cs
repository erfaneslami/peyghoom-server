using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Peyghoom.Core.Options;
using Peyghoom.Core.Results;
using Peyghoom.Services.CacheService;

namespace Peyghoom.Services.AuthService;

public class AuthService: IAuthService
{
    private readonly ICacheService _cacheService;
    private readonly TokenOption _tokenOption;
    
    public AuthService(ICacheService cacheService, IOptionsSnapshot<TokenOption> optionsSnapshot)
    {
        _cacheService = cacheService;
        _tokenOption = optionsSnapshot.Value;
    }

    public Result SendOto(long phoneNumber)
    {
        var otp = _generateRandomOtp();
        var cacheResult = _cacheService.CachePhoneNumberOtp(phoneNumber, otp);
        if (cacheResult.IsFailure) return Result.Failure(cacheResult.Error);
        // TODO: send otp to user phone
        Console.WriteLine($"User otp is {otp}");

        return Result.Success();
    }

    public Result<string> GenerateOtpToken(long phoneNumber)
    {
        int.TryParse(_tokenOption.OtpExpire, out var otpExpireMin);
        var expireDate = DateTime.Now.AddMinutes(otpExpireMin);
        var claims = new List<Claim>()
        {
            new ("purpose", "otp"),
            new ("phone_number", phoneNumber.ToString())
        };
        return _generateToken(expireDate, claims);
        
    }

    public Result<string> GenerateRegisterToken(long phoneNumber)
    {
        int.TryParse(_tokenOption.RegisterExpire, out var registerExpire);
        var expireDate = DateTime.Now.AddMinutes(registerExpire);
        var claims = new List<Claim>()
        {
            new ("purpose", "register"),
            new ("phone_number", phoneNumber.ToString())
        };
        return _generateToken(expireDate, claims);
        
    }

    public Result<string> GenerateAccessToken(long phoneNumber)
    {
        throw new NotImplementedException();
    }

    public Result<string> GenerateRefreshToken(long phoneNumber)
    {
        throw new NotImplementedException();
    }


    public Result ValidateOtp(long phoneNumber, string otp)
    {
        var otpCacheInfoResult = _cacheService.GetOtpInfo(phoneNumber);
        if (otpCacheInfoResult.IsFailure) return Result.Failure(otpCacheInfoResult.Error);

        if (otpCacheInfoResult.Value.Otp == otp)
        {
            return Result.Success();
        }
        else
        {
            return Result.Failure(Error.Conflict("Wrong OTP, please check your code and try again"));
        }
    }

    public Result<bool> IsUserRegistered(long phoneNumber)
    {
        throw new NotImplementedException();
    }

    public void RetrievePhoneNumber(string otpToken)
    {
        throw new NotImplementedException();
    }

    private string _generateRandomOtp()
    {
        var random = new Random();
        int otp = random.Next(10000, 99999);
        return otp.ToString();
    }

    private string _generateToken(DateTime expireDate, IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOption.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
           issuer: _tokenOption.Issuer,
           audience: null,
           claims: claims,
           notBefore: null,
           expires: expireDate, 
           signingCredentials: creds
        );


        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}