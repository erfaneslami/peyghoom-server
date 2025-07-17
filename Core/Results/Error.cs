namespace Peyghoom.Core.Results;

public record Error
{

    private string? Code { get; }
    private string? Message { get; }
    private ErrorType ErrorType { get; }
    
    private Error(string? code, string? message, ErrorType errorType)
    {
        Code = code;
        Message = message;
        ErrorType = errorType;
    }
    
    
    
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided", ErrorType.Failure);

    public static Error NotFound(string message = "The entity was not found!",string code = "1" ) =>
        new (code, message, ErrorType.NotFound);
    
    public static Error Validation( string message = "Bad Request", string code = "2") =>
        new (code, message, ErrorType.Validation);
    
    public static Error Conflict( string message, string code = "3") =>
        new (code, message, ErrorType.Conflict);
}