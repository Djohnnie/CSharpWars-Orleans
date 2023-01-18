using CSharpWars.Enums;
using Orleans;

namespace CSharpWars.Orleans.Contracts.Model;

[GenerateSerializer]
public class Vision
{
    [Id(0)]
    public IList<Bot> Bots { get; set; } = new List<Bot>();
    [Id(1)]
    public IList<Bot> FriendlyBots { get; set; } = new List<Bot>();
    [Id(2)]
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
                if (bot.PlayerName.ToLower() == botProperties.PlayerName.ToLower())
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