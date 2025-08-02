namespace Peyghoom.Services.CacheService.Models;

public class OtpCacheInfo
{
    public required long PhoneNumber { get; init; }
    public required long Otp { get; init; }
    public required DateTime CreateAt { get; init; } 
}