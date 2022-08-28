using CSharpWars.Enums;

namespace CSharpWars.Orleans.Contracts;

public record MoveDto
{
    public DateTime TimeStamp { get; init; }
    public string Owner { get; init; }
    public Move Move { get; init; }
    public string Description { get; init; }
    public string? Target { get; init; }
}