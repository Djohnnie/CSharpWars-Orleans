using CSharpWars.Enums;
using CSharpWars.Orleans.Contracts.Model;

namespace CSharpWars.Scripting.Moves;

public class TurnLeft : BaseMove
{
    public TurnLeft(BotProperties botProperties) : base(botProperties) { }

    public override BotResult Go()
    {
        // Build result based on current properties.
        var botResult = BotResult.Build(BotProperties);

        botResult.Move = Move.TurningLeft;

        switch (BotProperties.Orientation)
        {
            case Orientation.North:
                botResult.Orientation = Orientation.West;
                break;
            case Orientation.East:
                botResult.Orientation = Orientation.North;
                break;
            case Orientation.South:
                botResult.Orientation = Orientation.East;
                break;
            case Orientation.West:
                botResult.Orientation = Orientation.South;
                break;
        }

        return botResult;
    }
}