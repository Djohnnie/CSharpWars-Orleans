using CSharpWars.Enums;
using Orleans;

namespace CSharpWars.Orleans.Contracts;

[GenerateSerializer]
public class MoveDto
{
    [Id(0)]
    public DateTime TimeStamp { get; init; }
    [Id(1)]
    public string Owner { get; init; }
    [Id(2)]
    public Move Move { get; init; }
    [Id(3)]
    public string Description { get; init; }
    [Id(4)]
    public string? Target { get; init; }
}