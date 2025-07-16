namespace Peyghoom.Services.CacheService.Models;

public class OtpCacheInfo
{
    public required long PhoneNumber { get; init; }
    public required string Otp { get; init; }
    public required DateTime CreateAt { get; init; } 
}