using CSharpWars.Enums;

namespace CSharpWars.Scripting;

public class MoveComparer : IComparer<Move>
{
    public static MoveComparer Default => new();

    private MoveComparer() { }

    public int Compare(Move x, Move y)
    {
        return MoveWeight(x).CompareTo(MoveWeight(y));

        static int MoveWeight(Move move)
        {
            return move switch
            {
                Move.Idling => 0,
                Move.Died => 0,
                Move.ScriptError => 0,
                Move.RangedAttack => 1,
                Move.MeleeAttack => 2,
                Move.SelfDestruct => 3,
                Move.Teleport => 4,
                Move.WalkForward => 5,
                Move.TurningLeft => 6,
                Move.TurningRight => 6,
                Move.TurningAround => 6,
                _ => 0
            };
        }
    }
}