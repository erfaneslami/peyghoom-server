namespace Peyghoom.Core.Options;

public class ConnectionStringsOptions
{
    
    public const string ConnectionString = "ConnectionStrings";

    public string PeyghoomMongoDb { get; set; } = default!;
    public string DatabaseName{ get; set; } = default!;
}