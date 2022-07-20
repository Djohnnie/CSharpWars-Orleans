namespace CSharpWars.WebApi.Security;


public interface IPlayerContext
{
    string? PlayerName { get; set; }
}

public class PlayerContext : IPlayerContext
{
    public string? PlayerName { get; set; }
}