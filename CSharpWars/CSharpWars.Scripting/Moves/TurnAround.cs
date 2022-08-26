using CSharpWars.Enums;
using CSharpWars.Scripting.Model;

namespace CSharpWars.Scripting.Moves;

public class TurnAround : BaseMove
{
    public TurnAround(BotProperties botProperties) : base(botProperties) { }

    public override BotResult Go()
    {
        // Build result based on current properties.
        var botResult = BotResult.Build(BotProperties);

        botResult.Move = Move.TurningAround;

        switch (BotProperties.Orientation)
        {
            case Orientation.North:
                botResult.Orientation = Orientation.South;
                break;
            case Orientation.East:
                botResult.Orientation = Orientation.West;
                break;
            case Orientation.South:
                botResult.Orientation = Orientation.North;
                break;
            case Orientation.West:
                botResult.Orientation = Orientation.East;
                break;
        }

        return botResult;
    }
}