using Orleans;

namespace CSharpWars.Orleans.Contracts;

[GenerateSerializer]
public class ValidatedScriptDto
{
    [Id(0)]
    public long CompilationTimeInMilliseconds { get; init; }
    [Id(1)]
    public long RunTimeInMilliseconds { get; init; }

    [Id(2)]
    public List<ValidatedScriptMessageDto> ValidationMessages { get; init; }
}