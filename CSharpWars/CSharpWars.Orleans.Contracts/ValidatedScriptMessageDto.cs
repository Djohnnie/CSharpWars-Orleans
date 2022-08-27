namespace CSharpWars.Orleans.Contracts;

public record ValidatedScriptMessageDto
{
    public int LocationStart { get; init; }
    public int LocationEnd { get; init; }
    public string Message { get; init; }
}