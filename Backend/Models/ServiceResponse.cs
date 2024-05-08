namespace Ewidencjomat.Models;

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public int StatusCode { get; set; } = 200;
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
}