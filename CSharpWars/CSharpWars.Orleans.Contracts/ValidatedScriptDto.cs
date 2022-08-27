namespace CSharpWars.Orleans.Contracts;

public record ValidatedScriptDto
{
    public long CompilationTimeInMilliseconds { get; init; }
    public long RunTimeInMilliseconds { get; init; }

    public List<ValidatedScriptMessageDto> ValidationMessages { get; init; }
}