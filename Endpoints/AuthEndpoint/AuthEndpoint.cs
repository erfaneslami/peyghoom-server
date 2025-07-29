using System.Security.Claims;
using MongoDB.Bson;
using Peyghoom.Core.Results;
using Peyghoom.Endpoints.AuthEndpoint.Contracts;
using Peyghoom.Entities;
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

        
        auth.MapPost("/verification-code/verify", async (VerifyOptRequest request, HttpContext httpContext, IAuthService authService, IUserRepository userRepository) =>
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
                    registerationToken = registrationTokenResult,
                });
            }
            else
            {
                var accessTokenResult = authService.GenerateAccessToken(user);
                var refreshTokenResult = authService.GenerateRefreshToken();
                
                if (accessTokenResult.IsFailure) return accessTokenResult.ToProblemDetail(); 
                if (refreshTokenResult.IsFailure) return refreshTokenResult.ToProblemDetail();
                
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
            
            return Results.Ok(new
            {
                accessToken = accessTokenResult.Value,
                refreshToken = refreshTokenResult.Value,
            });
            
        }).RequireAuthorization("REGISTER");
    }
}