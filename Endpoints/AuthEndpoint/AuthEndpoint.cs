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

        
        auth.MapPost("/verification-code/validate", () =>
        {
            return Results.Ok("test");
        });
 
    }
}