using CSharpWars.Enums;

namespace CSharpWars.Orleans.Contracts.Model;

[GenerateSerializer]
public class Bot
{
    [Id(0)]
    public Guid Id { get; set; }
    [Id(1)]
    public string Name { get; set; }
    [Id(2)]
    public string PlayerName { get; set; }
    [Id(3)]
    public int X { get; set; }
    [Id(4)]
    public int Y { get; set; }
    [Id(5)]
    public Orientation Orientation { get; set; }

    private Bot() { }

    public static Bot Build(BotDto bot)
    {
        return new Bot
        {
            Id = bot.BotId,
            Name = bot.BotName,
            PlayerName = bot.PlayerName,
            X = bot.X,
            Y = bot.Y,
            Orientation = bot.Orientation
        };
    }

    public void Update(BotDto bot)
    {
        X = bot.X;
        Y = bot.Y;
        Orientation = bot.Orientation;
    }
}