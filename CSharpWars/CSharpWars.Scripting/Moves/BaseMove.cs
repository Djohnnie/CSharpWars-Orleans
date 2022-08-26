using CSharpWars.Common.Helpers;
using CSharpWars.Enums;
using CSharpWars.Scripting.Model;

namespace CSharpWars.Scripting.Moves;

public abstract class BaseMove
{
    private static readonly Dictionary<Move, Func<BotProperties, IRandomHelper, BaseMove>> _moves = new()
    {
        { Move.WalkForward, (p, rh) => new WalkForward(p) },
        { Move.TurningLeft, (p, rh) => new TurnLeft(p) },
        { Move.TurningRight, (p, rh) => new TurnRight(p) },
        { Move.TurningAround, (p, rh) => new TurnAround(p) },
        { Move.Teleport, (p, rh) => new Teleport(p, rh) },
        { Move.MeleeAttack, (p, rh) => new MeleeAttack(p) },
        { Move.RangedAttack, (p, rh) => new RangedAttack(p) },
        { Move.SelfDestruct, (p, rh) => new SelfDestruct(p) },
        { Move.Idling, (p, rh) => new EmptyMove(p) },
        { Move.ScriptError, (p, rh) => new EmptyMove(p) },
    };

    protected readonly BotProperties BotProperties;

    protected BaseMove(BotProperties botProperties)
    {
        BotProperties = botProperties;
    }

    public static BaseMove Build(BotProperties botProperties, IRandomHelper randomHelper)
    {
        return _moves[botProperties.CurrentMove](botProperties, randomHelper);
    }

    public abstract BotResult Go();

    protected bool WillCollide(int x, int y)
    {
        var collidingEdge = x < 0 || x >= BotProperties.Width || y < 0 || y >= BotProperties.Height;
        var collidingBot = BotProperties.Bots.FirstOrDefault(b => b.X == x && b.Y == y);
        return collidingBot != null || collidingEdge;
    }
}