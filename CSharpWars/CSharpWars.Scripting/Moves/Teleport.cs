using CSharpWars.Common.Helpers;
using CSharpWars.Enums;
using CSharpWars.Orleans.Contracts.Model;

namespace CSharpWars.Scripting.Moves;

public class Teleport : BaseMove
{
    private readonly IRandomHelper _randomHelper;

    public Teleport(BotProperties botProperties, IRandomHelper randomHelper) : base(botProperties)
    {
        _randomHelper = randomHelper;
    }

    public override BotResult Go()
    {
        // Build result based on current properties.
        var botResult = BotResult.Build(BotProperties);

        // Only perform move if enough stamina is available.
        if (BotProperties.CurrentStamina - Constants.STAMINA_ON_TELEPORT >= 0)
        {

            NormalizeDestination();

            var victimizedBot = FindVictimizedBot();
            if (victimizedBot != null)
            {
                botResult.Teleport(victimizedBot.Id, botResult.X, botResult.Y);
            }

            botResult.CurrentStamina -= Constants.STAMINA_ON_TELEPORT;
            botResult.Move = Move.Teleport;
            botResult.X = BotProperties.MoveDestinationX;
            botResult.Y = BotProperties.MoveDestinationY;
        }

        return botResult;
    }

    private void NormalizeDestination()
    {
        if (BotProperties.MoveDestinationX < 0 || BotProperties.MoveDestinationX > BotProperties.Width - 1)
        {
            BotProperties.MoveDestinationX = _randomHelper.Get(BotProperties.Width);
        }

        if (BotProperties.MoveDestinationY < 0 || BotProperties.MoveDestinationY > BotProperties.Height - 1)
        {
            BotProperties.MoveDestinationY = _randomHelper.Get(BotProperties.Height);
        }
    }

    private Bot FindVictimizedBot()
    {
        return BotProperties.Bots.FirstOrDefault(bot => bot.X == BotProperties.MoveDestinationX && bot.Y == BotProperties.MoveDestinationY);
    }
}