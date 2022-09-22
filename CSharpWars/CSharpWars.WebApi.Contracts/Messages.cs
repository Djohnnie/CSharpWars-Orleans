namespace CSharpWars.WebApi.Contracts;

public record GetAllMessagesRequest(string ArenaName);

public record GetAllMessagesResponse(List<Message> Messages);

public record Message
{
    public DateTime TimeStamp { get; init; }
    public string Owner { get; init; }
    public string Text { get; init; }
}