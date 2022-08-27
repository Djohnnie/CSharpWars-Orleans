using CSharpWars.Orleans.Contracts.Model;

namespace CSharpWars.Scripting.Moves;

public class EmptyMove : BaseMove
{
    public EmptyMove(BotProperties botProperties) : base(botProperties) { }

    public override BotResult Go()
    {
        // Build result based on current properties.
        var botResult = BotResult.Build(BotProperties);

        botResult.Move = BotProperties.CurrentMove;

        return botResult;
    }
}