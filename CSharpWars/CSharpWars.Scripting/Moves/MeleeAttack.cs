using CSharpWars.Enums;
using CSharpWars.Orleans.Contracts.Model;

namespace CSharpWars.Scripting.Moves;

public class MeleeAttack : BaseMove
{
    public MeleeAttack(BotProperties botProperties) : base(botProperties) { }

    public override BotResult Go()
    {
        // Build result based on current properties.
        var botResult = BotResult.Build(BotProperties);

        var victimizedBot = FindVictimizedBot();
        if (victimizedBot != null)
        {
            var backstab = victimizedBot.Orientation == BotProperties.Orientation;
            botResult.InflictDamage(victimizedBot.Id, backstab ? Constants.MELEE_BACKSTAB_DAMAGE : Constants.MELEE_DAMAGE);
        }

        botResult.Move = Move.MeleeAttack;

        return botResult;
    }

    private Bot FindVictimizedBot()
    {
        var neighbourLocation = GetNeighbourLocation();
        return BotProperties.Bots.FirstOrDefault(bot => bot.X == neighbourLocation.X && bot.Y == neighbourLocation.Y);
    }

    private (int X, int Y) GetNeighbourLocation()
    {
        switch (BotProperties.Orientation)
        {
            case Orientation.North:
                return (BotProperties.X, BotProperties.Y - 1);
            case Orientation.East:
                return (BotProperties.X + 1, BotProperties.Y);
            case Orientation.South:
                return (BotProperties.X, BotProperties.Y + 1);
            case Orientation.West:
                return (BotProperties.X - 1, BotProperties.Y);
        }

        return (-1, -1);
    }
}