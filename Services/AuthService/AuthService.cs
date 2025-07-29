using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Peyghoom.Core.Options;
using Peyghoom.Core.Results;
using Peyghoom.Endpoints.AuthEndpoint.Contracts;
using Peyghoom.Entities;
using Peyghoom.Repositories.UserRepository;
using Peyghoom.Services.CacheService;

namespace Peyghoom.Services.AuthService;

public class AuthService: IAuthService
{
    private readonly ICacheService _cacheService;
    private readonly TokenOption _tokenOption;
    private readonly IUserRepository _userRepository;
    
    public AuthService(ICacheService cacheService, IOptionsSnapshot<TokenOption> optionsSnapshot, IUserRepository userRepository)
    {
        _cacheService = cacheService;
        _userRepository = userRepository;
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

    public async Task<Result<User>> RegisterUserAsync(RegisterUserCommand registerUserCommand)
    {
        var user = await _userRepository.CreateUserAsync(new User()
        {
            UserName = registerUserCommand.UserName,
            FirstName = registerUserCommand.FirstName,
            LastName = registerUserCommand.LastName,
            PhoneNumber = registerUserCommand.PhoneNumber,
        });

        return user;
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

    public Result<string> GenerateAccessToken(User user)
    {
        int.TryParse(_tokenOption.AccessTokenExpire, out var accessTokenExpire);
        var expireDate = DateTime.Now.AddMinutes(accessTokenExpire);
        var claims = new List<Claim>()
        {
            new ("purpose", "user"),
            new ("phone_number", user.PhoneNumber.ToString()),
            new ("sub", user.Id.ToString()),
            new ("user_name", user.UserName),
        };
        return _generateToken(expireDate, claims);
    }

    public Result<string> GenerateRefreshToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
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