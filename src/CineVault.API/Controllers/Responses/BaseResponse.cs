namespace CineVault.API.Controllers.Responses;

public class BaseResponse
{
    public required int StatusCode { get; set; }
    public required bool Success { get; set; }
    public required string Message { get; set; }
    public DateTime ResponseDate { get; } = DateTime.UtcNow;
    public Dictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();

    public static BaseResponse Ok(string message)
    {
        return new BaseResponse
        {
            StatusCode = 200,
            Success = true,
            Message = message
        };
    }

    public static BaseResponse Ok<T>(T data, string message)
    {
        return new BaseResponse<T>
        {
            Data = data,
            StatusCode = 200,
            Success = true,
            Message = message
        };
    }

    public static BaseResponse Created(string message)
    {
        return new BaseResponse
        {
            StatusCode = 201,
            Success = true,
            Message = message
        };
    }

    public static BaseResponse NotFound(string message)
    {
        return new BaseResponse
        {
            StatusCode = 404,
            Success = false,
            Message = message
        };
    }
}

public sealed class BaseResponse<T> : BaseResponse
{
    public required T Data { get; set; }
}