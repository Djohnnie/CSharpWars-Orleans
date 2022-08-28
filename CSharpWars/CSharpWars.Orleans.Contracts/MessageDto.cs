namespace CSharpWars.Orleans.Contracts;

public record MessageDto
{
    public DateTime TimeStamp { get; init; }
    public string Owner { get; init; }
    public string Message { get; init; }
}