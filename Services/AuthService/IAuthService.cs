using MongoDB.Bson;
using Peyghoom.Core.Results;
using Peyghoom.Endpoints.AuthEndpoint.Contracts;
using Peyghoom.Entities;

namespace Peyghoom.Services.AuthService;

public interface IAuthService
{
   public Result SendOto(long phoneNumber);
   public Task<Result<User>> RegisterUserAsync(RegisterUserCommand registerUserCommand);
   public Result<string> GenerateOtpToken(long phoneNumber);
   public Result<string> GenerateRegisterToken(long phoneNumber);
   public Result<string> GenerateAccessToken(User user);
   public Result<string> GenerateRefreshToken();
   public Task<Result<RefreshToken>> StoreRefreshTokenAsync(string token, ObjectId userId);
   public Result ValidateOtp(long phoneNumber, string otp);
}

public class RegisterUserCommand
{
   public required string UserName { get; set; }
   public required string FirstName { get; set; }
   public required string LastName { get; set; }
   public required long   PhoneNumber { get; set; }
}