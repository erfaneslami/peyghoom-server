namespace Peyghoom.Core.Options;

public class TokenOption
{
    public const string Token = "Token";
    public required string Issuer { get; init; }
    public required string SecretKey { get; init; }
    public required string OtpExpire { get; init; }
    public required string RegisterExpire { get; init; }
    public required string AccessTokenExpire { get; init; }
    public required string RefreshTokenExpireDays { get; init; }
    
}