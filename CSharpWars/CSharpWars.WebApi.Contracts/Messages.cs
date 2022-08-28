namespace CSharpWars.WebApi.Contracts;

public record GetAllMessagesRequest(string ArenaName);

public record GetAllMessagesResponse(List<Message> Messages);

public record Message
{
    public string Text { get; init; }
}