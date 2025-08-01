using System.Security.Claims;
using MongoDB.Bson;
using Peyghoom.Core.Results;
using Peyghoom.Endpoints.AuthEndpoint.Contracts;
using Peyghoom.Entities;
using Peyghoom.Repositories.AuthRepository;
using Peyghoom.Repositories.UserRepository;
using Peyghoom.Services.AuthService;

namespace Peyghoom.Endpoints.AuthEndpoint;

public class AuthEndpoint: IEndpointGroup
{
    public void MapEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var auth = endpointRouteBuilder.MapGroup("auth/");



        auth.MapPost("/verification-code", (OtpRequest request, IAuthService authService) =>
        {
            var sendOtpResult = authService.SendOto(request.PhoneNumber);
            if (sendOtpResult.IsFailure) return sendOtpResult.ToProblemDetail();
            
            var tokenResult = authService.GenerateOtpToken(request.PhoneNumber);
            if (tokenResult.IsFailure) return tokenResult.ToProblemDetail();
            
            return Results.Ok(new
            {
                token = tokenResult.Value
            });

        });

        
        auth.MapPost("/verification-code/verify", async (VerifyOptRequest request, HttpContext httpContext,HttpResponse httpResponse, IAuthService authService, IUserRepository userRepository) =>
        {
            var phoneNumberValue = httpContext.User.FindFirstValue("phone_number");
            long.TryParse(phoneNumberValue, out var phoneNumber);
           
            var validateOtpResult = authService.ValidateOtp(phoneNumber, request.Code);
            if (validateOtpResult.IsFailure) return validateOtpResult.ToProblemDetail();

            var user = await userRepository.GetUserByPhoneNumberAsync(phoneNumber);

            if (user == null)
            {
                var registrationTokenResult = authService.GenerateRegisterToken(phoneNumber);
                if (registrationTokenResult.IsFailure) return registrationTokenResult.ToProblemDetail();
                
                // TODO: return proper code or something for client to redirection 
                return Results.Ok(new
                {
                    registerationToken = registrationTokenResult.Value,
                });
            }
            else
            {
                var accessTokenResult = authService.GenerateAccessToken(user);
                var refreshTokenResult = authService.GenerateRefreshToken();

                
                if (accessTokenResult.IsFailure) return accessTokenResult.ToProblemDetail(); 
                if (refreshTokenResult.IsFailure) return refreshTokenResult.ToProblemDetail();
                
                var storeRefResult = await authService.StoreRefreshTokenAsync(refreshTokenResult.Value, user.Id);
                if (storeRefResult.IsFailure) return storeRefResult.ToProblemDetail();
                
                httpResponse.Cookies.Append("refreshToken", refreshTokenResult.Value, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // TODO: fix it in production
                    SameSite = SameSiteMode.Strict,
                    Expires = storeRefResult.Value.ExpireAt
                });
                
                return Results.Ok(new
                {
                    accessToken = accessTokenResult.Value,
                    refreshToken = refreshTokenResult.Value,
                });
            }
            
        }).RequireAuthorization("OTPVerify");

        auth.MapPost("register", async (RegisterRequest request,HttpContext httpContext, IAuthService authService ) =>
        {
            
            var phoneNumberValue = httpContext.User.FindFirstValue("phone_number");
            long.TryParse(phoneNumberValue, out var phoneNumber);

            var userResult = await authService.RegisterUserAsync(new RegisterUserCommand()
            {
                PhoneNumber = phoneNumber,
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
            });
            
            if (userResult.IsFailure) return userResult.ToProblemDetail(); 
            
            
            var accessTokenResult = authService.GenerateAccessToken(userResult.Value);
            var refreshTokenResult = authService.GenerateRefreshToken();
                
            if (accessTokenResult.IsFailure) return accessTokenResult.ToProblemDetail(); 
            if (refreshTokenResult.IsFailure) return refreshTokenResult.ToProblemDetail();
            
            var storeRefResult = await authService.StoreRefreshTokenAsync(refreshTokenResult.Value, userResult.Value.Id);
            if (storeRefResult.IsFailure) return storeRefResult.ToProblemDetail();
            
            return Results.Ok(new
            {
                accessToken = accessTokenResult.Value,
                refreshToken = refreshTokenResult.Value,
            });
            
        }).RequireAuthorization("REGISTER");

        auth.MapPost("refresh", async (HttpRequest httpRequest, HttpResponse httpResponse, IAuthRepository authRepository, IAuthService authService, IUserRepository userRepository, CancellationToken cancellationToken) =>
        {
            httpRequest.Cookies.TryGetValue("refreshToken", out var refreshToken);
            if (refreshToken == null)
                return Result.Failure(Error.Validation("refresh token is require")).ToProblemDetail();

            var getRefreshResult = await authRepository.FindRefreshByTokenAsync(refreshToken, cancellationToken);
            if (getRefreshResult.IsFailure) return getRefreshResult.ToProblemDetail();

            if (getRefreshResult.Value.IsRevoked || getRefreshResult.Value.ExpireAt < DateTime.Now)
            {
                return Result.Failure(Error.UnAuthorize()).ToProblemDetail();
            }

            getRefreshResult.Value.IsRevoked = true;

            var user = await userRepository.GetUserByIdAsync(getRefreshResult.Value.UserId.ToString(), cancellationToken);
            if (user == null) return Result.Failure(Error.NotFound("user not found")).ToProblemDetail();
            
            var accessTokenResult = authService.GenerateAccessToken(user);
            var refreshTokenResult = authService.GenerateRefreshToken();
                
            if (accessTokenResult.IsFailure) return accessTokenResult.ToProblemDetail(); 
            if (refreshTokenResult.IsFailure) return refreshTokenResult.ToProblemDetail();

            var storeRefResult = await authService.StoreRefreshTokenAsync(refreshTokenResult.Value, user.Id);
            var updateOldRefResult =
                await authRepository.UpdateRefreshTokenAsync(getRefreshResult.Value, cancellationToken);
            
            if (storeRefResult.IsFailure) return storeRefResult.ToProblemDetail(); 
            if (updateOldRefResult.IsFailure) return updateOldRefResult.ToProblemDetail();
            
            httpResponse.Cookies.Append("refreshToken", refreshTokenResult.Value, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // TODO: fix it in production
                SameSite = SameSiteMode.Strict,
                Expires = getRefreshResult.Value.ExpireAt
            });
            
            return Results.Ok(new
            {
                accessToken = accessTokenResult.Value,
                refreshToken = refreshTokenResult.Value,
            });

        });
    }
}