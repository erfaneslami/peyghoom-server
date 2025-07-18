
using Microsoft.AspNetCore.Mvc;

namespace Peyghoom.Core.Results;

public static class ResultExtention
{
   public static IResult ToProblemDetail(this Result result)
   {
      
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Can't convert success result to problem");
        }

        var problem = new ProblemDetails()
        {
            Status = GetStatusCode(result.Error.ErrorType),
            Title = GetTitle(result.Error.ErrorType),
            Type = GetType(result.Error.ErrorType),
            Extensions = new Dictionary<string, object?>
            {
                { "errors", new[] { result.Error } }
            },
        };
        
        
        return Microsoft.AspNetCore.Http.Results.Problem(
            detail: problem.Detail,
            statusCode: problem.Status,
            title: problem.Title,
            type: problem.Type,
            extensions: problem.Extensions
        );
   }
   
   
    static int GetStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,

            _ => StatusCodes.Status500InternalServerError
        };

    static string GetTitle(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => "Bad Request",
            ErrorType.NotFound => "Not found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Forbidden => "Forbidden",
            ErrorType.Unauthorized => "Unauthorized",

            _ => "Server failure"
        };

    static string GetType(ErrorType statusCode) =>
        statusCode switch
        {
            ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            ErrorType.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            ErrorType.Unauthorized => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
}