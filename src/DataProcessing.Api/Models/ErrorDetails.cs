namespace DataProcessing.Api.Models;

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }

    public string ToJsonString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}
