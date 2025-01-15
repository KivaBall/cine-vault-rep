namespace CineVault.API.Controllers.Requests;

public class BaseRequest
{
    public required string? SenderName { get; set; }
    public DateTime RequestTime { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();
}

public sealed class BaseRequest<T> : BaseRequest
{
    public required T Data { get; set; }
}