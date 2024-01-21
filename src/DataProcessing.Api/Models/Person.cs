namespace DataProcessing.Api.Models;

public record Person
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public DateTime DateOfBirth { get; init; }
    public bool IsActive { get; init; }
}
