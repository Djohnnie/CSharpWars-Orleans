using CSharpWars.Enums;
using CSharpWars.Scripting.Model;

namespace CSharpWars.Scripting.Moves;

public class RangedAttack : BaseMove
{
    public RangedAttack(BotProperties botProperties) : base(botProperties) { }

    public override BotResult Go()
    {
        // Build result based on current properties.
        var botResult = BotResult.Build(BotProperties);

        var victimizedBot = FindVictimizedBot();
        if (victimizedBot != null)
        {
            botResult.InflictDamage(victimizedBot.Id, Constants.RANGED_DAMAGE);
        }

        botResult.Move = Move.RangedAttack;

        return botResult;
    }

    private Bot FindVictimizedBot()
    {
        return BotProperties.Bots.FirstOrDefault(bot => bot.X == BotProperties.MoveDestinationX && bot.Y == BotProperties.MoveDestinationY);
    }
}