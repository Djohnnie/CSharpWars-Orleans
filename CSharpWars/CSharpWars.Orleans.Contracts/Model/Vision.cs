using CSharpWars.Enums;

namespace CSharpWars.Orleans.Contracts.Model;

public class Vision
{
    public IList<Bot> Bots { get; set; } = new List<Bot>();
    public IList<Bot> FriendlyBots { get; set; } = new List<Bot>();
    public IList<Bot> EnemyBots { get; set; } = new List<Bot>();

    private Vision() { }

    public static Vision Build(BotProperties botProperties)
    {
        var vision = new Vision();
        foreach (var bot in botProperties.Bots)
        {
            if (bot.Id != botProperties.BotId && BotIsVisible(bot, botProperties))
            {
                vision.Bots.Add(bot);
                if (bot.PlayerName == botProperties.PlayerName)
                {
                    vision.FriendlyBots.Add(bot);
                }
                else
                {
                    vision.EnemyBots.Add(bot);
                }
            }
        }

        return vision;
    }

    private static bool BotIsVisible(Bot bot, BotProperties botProperties)
    {
        var result = false;

        switch (botProperties.Orientation)
        {
            case Orientation.North:
                result = bot.Y < botProperties.Y;
                break;
            case Orientation.East:
                result = bot.X > botProperties.X;
                break;
            case Orientation.South:
                result = bot.Y > botProperties.Y;
                break;
            case Orientation.West:
                result = bot.X < botProperties.X;
                break;
        }

        return result;
    }
}