namespace CSharpWars.Enums;

public enum Move : byte
{
    Idling = 0,

    TurningLeft = 1,

    TurningRight = 2,

    TurningAround = 3,

    WalkForward = 4,

    Teleport = 5,

    MeleeAttack = 6,

    RangedAttack = 7,

    SelfDestruct = 8,

    Died = 9,

    ScriptError = 10
}