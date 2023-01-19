namespace CSharpWars.Orleans.Contracts;

[GenerateSerializer]
public class MessageDto
{
    [Id(0)]
    public DateTime TimeStamp { get; init; }
    [Id(1)]
    public string Owner { get; init; }
    [Id(2)]
    public string Message { get; init; }
}