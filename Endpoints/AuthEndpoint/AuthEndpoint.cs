using System.Security.Claims;
using Peyghoom.Core.Results;
using Peyghoom.Endpoints.AuthEndpoint.Contracts;
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

        
        auth.MapPost("/verification-code/verify", (VerifyOptRequest request, HttpContext httpContext, IAuthService authService) =>
        {
            var phoneNumberValue = httpContext.User.FindFirstValue("phone_number");
            int.TryParse(phoneNumberValue, out var phoneNumber);
           
            var validateOtpResult = authService.ValidateOtp(phoneNumber, request.Code);
            if (validateOtpResult.IsFailure) return validateOtpResult.ToProblemDetail();

            var isUserRegistered = authService.IsUserRegistered(phoneNumber);
            if (isUserRegistered.IsFailure) return isUserRegistered.ToProblemDetail();

            if (!isUserRegistered.Value)
            {
                var regToken = authService.GenerateRegisterToken(phoneNumber);
                // TODO: create user in database
                // TODO: redirect user to register page with regToken
            }
            else
            {
                // var accessToken = authService.GenerateAccessToken(phoneNumber);
                // TODO: create access refresh token and redirect user to main page
            }
            
            return Results.Ok("test");
        }).RequireAuthorization("OTPVerify");
 
    }
}