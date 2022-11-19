using Orleans;

namespace CSharpWars.Orleans.Contracts;

[GenerateSerializer]
public class ScriptToValidateDto
{
    [Id(0)]
    public string Script { get; init; }
}