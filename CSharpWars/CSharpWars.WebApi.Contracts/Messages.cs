namespace CSharpWars.WebApi.Contracts;

public class GetAllMessagesRequest
{
    public string ArenaName { get; set; }
}

public class GetAllMessagesResponse
{
    public List<Message> Messages { get; set; }
}

public class Message
{
    public DateTime TimeStamp { get; init; }
    public string Owner { get; init; }
    public string Text { get; init; }
}