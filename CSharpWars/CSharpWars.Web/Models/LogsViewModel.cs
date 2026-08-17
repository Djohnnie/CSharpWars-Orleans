namespace CSharpWars.Web.Models;

public class LogsViewModel
{
    public MoveLogsViewModel Moves { get; set; } = new();
    public MessageLogsViewModel Messages { get; set; } = new();
}