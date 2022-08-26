using CSharpWars.Enums;
using CSharpWars.Scripting.Model;

namespace CSharpWars.Scripting.Moves;

public class TurnRight : BaseMove
{
    public TurnRight(BotProperties botProperties) : base(botProperties) { }

    public override BotResult Go()
    {
        // Build result based on current properties.
        var botResult = BotResult.Build(BotProperties);

        botResult.Move = Move.TurningRight;

        switch (BotProperties.Orientation)
        {
            case Orientation.North:
                botResult.Orientation = Orientation.East;
                break;
            case Orientation.East:
                botResult.Orientation = Orientation.South;
                break;
            case Orientation.South:
                botResult.Orientation = Orientation.West;
                break;
            case Orientation.West:
                botResult.Orientation = Orientation.North;
                break;
        }

        return botResult;
    }
}