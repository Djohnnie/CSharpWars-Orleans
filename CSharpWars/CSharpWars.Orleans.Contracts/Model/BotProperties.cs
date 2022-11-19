using CSharpWars.Common.Extensions;
using CSharpWars.Enums;
using Orleans;

namespace CSharpWars.Orleans.Contracts.Model;

[GenerateSerializer]
public class BotProperties
{
    [Id(0)]
    public Guid BotId { get; private set; }
    [Id(1)]
    public string Name { get; private set; }
    [Id(2)]
    public string PlayerName { get; private set; }
    [Id(3)]
    public int Width { get; private set; }
    [Id(4)]
    public int Height { get; private set; }
    [Id(5)]
    public int X { get; private set; }
    [Id(6)]
    public int Y { get; private set; }
    [Id(7)]
    public Orientation Orientation { get; private set; }
    [Id(8)]
    public Move LastMove { get; private set; }
    [Id(9)]
    public int MaximumHealth { get; private set; }
    [Id(10)]
    public int CurrentHealth { get; private set; }
    [Id(11)]
    public int MaximumStamina { get; private set; }
    [Id(12)]
    public int CurrentStamina { get; private set; }
    [Id(13)]
    public Dictionary<string, string> Memory { get; private set; }
    [Id(14)]
    public List<Bot> Bots { get; set; }
    [Id(15)]
    public Move CurrentMove { get; set; }
    [Id(16)]
    public int MoveDestinationX { get; set; }
    [Id(17)]
    public int MoveDestinationY { get; set; }
    [Id(18)]
    public string Message { get; set; }

    private BotProperties() { }

    public void Update(BotDto bot)
    {
        CurrentHealth = bot.CurrentHealth;
        X = bot.X;
        Y = bot.Y;
    }

    public static BotProperties Build(BotDto bot, ArenaDto arena, IList<BotDto> bots)
    {
        return new BotProperties
        {
            BotId = bot.BotId,
            Name = bot.BotName,
            PlayerName = bot.PlayerName,
            Width = arena.Width,
            Height = arena.Height,
            X = bot.X,
            Y = bot.Y,
            Orientation = bot.Orientation,
            LastMove = bot.Move,
            MaximumHealth = bot.MaximumHealth,
            CurrentHealth = bot.CurrentHealth,
            MaximumStamina = bot.MaximumStamina,
            CurrentStamina = bot.CurrentStamina,
            Memory = bot.Memory.Deserialize<Dictionary<string, string>>() ?? new Dictionary<string, string>(),
            Bots = BuildBots(bots),
            CurrentMove = Move.Idling
        };
    }

    private static List<Bot> BuildBots(IList<BotDto> bots)
    {
        return bots.Select(Bot.Build).ToList();
    }
}