namespace Presentation.Responses;

public abstract class ResponseResult
{
    // Base class for service responses
    public bool Succeeded { get; set; }
    public int? StatusCode { get; set; }
    public string? Error { get; set; }
    public string? Message { get; set; }
}

public class UserResult : ResponseResult
{
    public string? Result { get; set; }
}

public class UserResult<T> : ResponseResult
{
    public T? Result { get; set; }
}