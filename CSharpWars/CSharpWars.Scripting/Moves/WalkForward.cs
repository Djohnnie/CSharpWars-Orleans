using CSharpWars.Enums;
using CSharpWars.Orleans.Contracts.Model;

namespace CSharpWars.Scripting.Moves;

public class WalkForward : BaseMove
{
    public WalkForward(BotProperties botProperties) : base(botProperties) { }

    public override BotResult Go()
    {
        // Build result based on current properties.
        var botResult = BotResult.Build(BotProperties);

        // Only perform move if enough stamina is available.
        if (BotProperties.CurrentStamina - Constants.STAMINA_ON_MOVE >= 0)
        {
            var destinationX = BotProperties.X;
            var destinationY = BotProperties.Y;

            switch (BotProperties.Orientation)
            {
                case Orientation.North:
                    destinationY--;
                    break;
                case Orientation.East:
                    destinationX++;
                    break;
                case Orientation.South:
                    destinationY++;
                    break;
                case Orientation.West:
                    destinationX--;
                    break;
            }

            if (!WillCollide(destinationX, destinationY))
            {
                botResult.CurrentStamina -= Constants.STAMINA_ON_MOVE;
                botResult.Move = Move.WalkForward;
                botResult.X = destinationX;
                botResult.Y = destinationY;
            }
        }

        return botResult;
    }
}