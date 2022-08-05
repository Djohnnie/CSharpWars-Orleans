using CSharpWars.Enums;

namespace CSharpWars.Scripting;

public class MoveComparer : IComparer<Move>
{
    private readonly Dictionary<Move, int> _weights = new()
    {
        { Move.Idling, 0 },
        { Move.Died, 0 },
        { Move.ScriptError, 0 },
        { Move.RangedAttack, 1 },
        { Move.MeleeAttack, 2 },
        { Move.SelfDestruct, 3 },
        { Move.Teleport, 4 },
        { Move.WalkForward, 5 },
        { Move.TurningLeft, 6 },
        { Move.TurningRight, 6 },
        { Move.TurningAround, 6 }
    };

    public int Compare(Move x, Move y)
    {
        return _weights[x].CompareTo(_weights[y]);
    }
}