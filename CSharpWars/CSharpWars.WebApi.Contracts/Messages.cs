namespace CSharpWars.WebApi.Contracts;

public class GetAllMessagesRequest
{
    public string ArenaName { get; set; } = string.Empty;
}

public class GetAllMessagesResponse
{
    public List<Message> Messages { get; set; } = [];
}

public class Message
{
    public DateTime TimeStamp { get; init; }
    public string Owner { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
}