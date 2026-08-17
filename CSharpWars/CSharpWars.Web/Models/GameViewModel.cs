namespace CSharpWars.Web.Models;

public class GameViewModel
{
    public string PlayerName { get; set; } = string.Empty;
    public string SampleScript { get; set; } = string.Empty;
    public bool IsCustomPlayEnabled { get; set; }
    public bool IsTemplatePlayEnabled { get; set; }
}