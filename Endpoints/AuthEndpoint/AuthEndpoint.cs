using Peyghoom.Endpoints.AuthEndpoint.Contracts;

namespace Peyghoom.Endpoints.AuthEndpoint;

public class AuthEndpoint: IEndpointGroup
{
    public void MapEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var auth = endpointRouteBuilder.MapGroup("auth/");



        auth.MapPost("/otp", (OtpRequest request) =>
        {
            return Results.Ok("test");
            
        });

        
        auth.MapPost("/otp/validate", () =>
        {
            return Results.Ok("test");
        });

    }
}