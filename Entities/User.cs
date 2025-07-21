namespace Peyghoom.Entities;

public class User
{
    public long Id { get; set; }
    public DateTime CreateAt { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime BirthDay { get; set; }
    public long PhoneNumber { get; set; }
}