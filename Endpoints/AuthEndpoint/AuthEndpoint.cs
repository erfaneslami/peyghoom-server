namespace Peyghoom.Endpoints.AuthEndpoint;

public class AuthEndpoint: IEndpointGroup
{
    public void MapEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var auth = endpointRouteBuilder.MapGroup("auth/");


        auth.MapPost("/login", () =>
        {
            return Results.Ok("test");
        });

        
        auth.MapPost("/register", () =>
        {
            return Results.Ok("test");
        });
    }
}