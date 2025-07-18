namespace Peyghoom.Endpoints.AuthEndpoint.Contracts;

public class OtpRequest
{
    public long PhoneNumber { get; set; }    
    public int CountryCode { get; set; }
}