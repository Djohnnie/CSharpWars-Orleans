namespace CSharpWars.Web.Models;

public class PlayViewModel
{
    public string HappyMessage { get; set; } = string.Empty;
    public string SadMessage { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public string BotName { get; set; } = string.Empty;
    public int BotHealth { get; set; }
    public int BotStamina { get; set; }
    public string Script { get; set; } = string.Empty;
    public Guid SelectedScript { get; set; }
    public IList<Template> Scripts { get; set; } = [];
}