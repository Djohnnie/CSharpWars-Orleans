using Orleans;

namespace CSharpWars.Orleans.Contracts;

[GenerateSerializer]
public class ValidatedScriptMessageDto
{
    [Id(0)]
    public int LocationStart { get; init; }
    [Id(1)]
    public int LocationEnd { get; init; }
    [Id(2)]
    public string Message { get; init; }
}