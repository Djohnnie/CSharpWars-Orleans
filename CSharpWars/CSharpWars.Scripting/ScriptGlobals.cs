using CSharpWars.Common.Extensions;
using CSharpWars.Enums;
using CSharpWars.Orleans.Contracts.Model;
using System.ComponentModel;

namespace CSharpWars.Scripting;

public class ScriptGlobals
{
    #region <| Private Members |>

    private readonly BotProperties _50437079C366407D978Fe4Afd60C535F;
    private readonly Vision _vision;

    #endregion

    #region <| Public Properties |>

    [Description("Gets the width of the current arena.")]
    public int Width => _50437079C366407D978Fe4Afd60C535F.Width;
   
    [Description("Gets the height of the current arena.")]
    public int Height => _50437079C366407D978Fe4Afd60C535F.Height;

    [Description("Gets the current robot X-location on the arena.")]
    public int X => _50437079C366407D978Fe4Afd60C535F.X;

    [Description("Gets the current robot Y-location on the arena.")]
    public int Y => _50437079C366407D978Fe4Afd60C535F.Y;

    [Description("Gets the current robot orientation.")]
    public Orientation Orientation => _50437079C366407D978Fe4Afd60C535F.Orientation;

    [Description("Gets the last move that was executed by the robot.")]
    public Move LastMove => _50437079C366407D978Fe4Afd60C535F.LastMove;

    [Description("Gets the maximum available health for the robot.")]
    public int MaximumHealth => _50437079C366407D978Fe4Afd60C535F.MaximumHealth;

    [Description("Gets the remaining health for the robot.")]
    public int CurrentHealth => _50437079C366407D978Fe4Afd60C535F.CurrentHealth;

    [Description("Gets the maximum available stamina for the robot.")]
    public int MaximumStamina => _50437079C366407D978Fe4Afd60C535F.MaximumStamina;

    [Description("Gets the remaining stamina for the robot.")]
    public int CurrentStamina => _50437079C366407D978Fe4Afd60C535F.CurrentStamina;

    [Description("Gets the vision for the robot.")]
    public Vision Vision => _vision;

    #endregion

    #region <| Constants |>

    public const int MELEE_DAMAGE = Constants.MELEE_DAMAGE;
    public const int MELEE_BACKSTAB_DAMAGE = Constants.MELEE_BACKSTAB_DAMAGE;
    public const int RANGED_DAMAGE = Constants.RANGED_DAMAGE;
    public const int SELF_DESTRUCT_MAX_DAMAGE = Constants.SELF_DESTRUCT_MAX_DAMAGE;
    public const int SELF_DESTRUCT_MED_DAMAGE = Constants.SELF_DESTRUCT_MED_DAMAGE;
    public const int SELF_DESTRUCT_MIN_DAMAGE = Constants.SELF_DESTRUCT_MIN_DAMAGE;
    public const int MAXIMUM_RANGE = Constants.MAXIMUM_RANGE;
    public const int MAXIMUM_TELEPORT = Constants.MAXIMUM_TELEPORT;
    public const int STAMINA_ON_MOVE = Constants.STAMINA_ON_MOVE;
    public const int STAMINA_ON_TELEPORT = Constants.STAMINA_ON_TELEPORT;

    public const Move IDLING = Move.Idling;
    public const Move TURNING_LEFT = Move.TurningLeft;
    public const Move TURNING_RIGHT = Move.TurningRight;
    public const Move TURNING_AROUND = Move.TurningAround;
    public const Move MOVING_FORWARD = Move.WalkForward;
    public const Move RANGED_ATTACK = Move.RangedAttack;
    public const Move MELEE_ATTACK = Move.MeleeAttack;
    public const Move SELF_DESTRUCTING = Move.SelfDestruct;
    public const Move SCRIPT_ERROR = Move.ScriptError;
    public const Move DYING = Move.Died;
    public const Move TELEPORTING = Move.Teleport;

    public const Orientation NORTH = Orientation.North;
    public const Orientation EAST = Orientation.East;
    public const Orientation SOUTH = Orientation.South;
    public const Orientation WEST = Orientation.West;

    #endregion

    #region <| Construction |>

    private ScriptGlobals(BotProperties botProperties)
    {
        _50437079C366407D978Fe4Afd60C535F = botProperties;
        _vision = Vision.Build(botProperties);
    }

    public static ScriptGlobals Build(BotProperties botProperties)
    {
        return new ScriptGlobals(botProperties);
    }

    #endregion

    #region <| Public Methods |>

    /// <summary>
    /// Calling this method will move the player one position forward.
    /// </summary>
    [Description("Calling this method will move the player one position forward.")]
    public void WalkForward()
    {
        SetCurrentMove(Move.WalkForward);
    }

    /// <summary>
    /// Calling this method will turn the player 90 degrees to the left.
    /// </summary>
    [Description("Calling this method will turn the player 90 degrees to the left.")]
    public void TurnLeft()
    {
        SetCurrentMove(Move.TurningLeft);
    }

    /// <summary>
    /// Calling this method will turn the player 90 degrees to the right.
    /// </summary>
    [Description("Calling this method will turn the player 90 degrees to the right.")]
    public void TurnRight()
    {
        SetCurrentMove(Move.TurningRight);
    }

    /// <summary>
    /// Calling this method will turn the player 180 degrees around.
    /// </summary>
    [Description("Calling this method will turn the player 180 degrees around.")]
    public void TurnAround()
    {
        SetCurrentMove(Move.TurningAround);
    }

    /// <summary>
    /// Calling this method will self destruct the player resulting in its death.
    /// </summary>
    [Description("Calling this method will self destruct the player resulting in its death.")]
    public void SelfDestruct()
    {
        SetCurrentMove(Move.SelfDestruct);
    }

    /// <summary>
    /// Calling this method will execute a melee attack.
    /// </summary>
    [Description("Calling this method will execute a melee attack.")]
    public void MeleeAttack()
    {
        SetCurrentMove(Move.MeleeAttack);
    }

    /// <summary>
    /// Calling this method will execute a ranged attack to the specified location.
    /// </summary>
    [Description("Calling this method will execute a ranged attack to the specified location.")]
    public void RangedAttack(int x, int y)
    {
        SetCurrentMove(Move.RangedAttack, x, y);
    }

    /// <summary>
    /// Calling this method will teleport the player to the specified location.
    /// </summary>
    [Description("Calling this method will teleport the player to the specified location.")]
    public void Teleport(int x, int y)
    {
        SetCurrentMove(Move.Teleport, x, y);
    }

    /// <summary>
    /// Calling this method will store information into the players memory.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    [Description("Calling this method will store information into the players memory.")]
    public void StoreInMemory<T>(string key, T value)
    {
        if (_50437079C366407D978Fe4Afd60C535F.Memory.ContainsKey(key))
        {
            _50437079C366407D978Fe4Afd60C535F.Memory[key] = value.Serialize();
        }
        else
        {
            _50437079C366407D978Fe4Afd60C535F.Memory.Add(key, value.Serialize());
        }
    }

    /// <summary>
    /// Calling this method will load information from the players memory.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    [Description("Calling this method will load information from the players memory.")]
    public T? LoadFromMemory<T>(string key)
    {
        if (_50437079C366407D978Fe4Afd60C535F.Memory.TryGetValue(key, out string? value) 
            && value is not null)
        {
            return value.Deserialize<T>();
        }

        return default;
    }

    /// <summary>
    /// Calling this method will remove information from the players memory.
    /// </summary>
    /// <param name="key"></param>
    [Description("Calling this method will remove information from the players memory.")]
    public void RemoveFromMemory(string key)
    {
        _50437079C366407D978Fe4Afd60C535F.Memory.Remove(key);
    }

    /// <summary>
    /// Calling this method will make the player talk.
    /// </summary>
    /// <param name="message"></param>
    public void Talk(string message)
    {
        _50437079C366407D978Fe4Afd60C535F.Message = message;
    }

    #endregion

    #region <| Helper Methods |>

    private bool SetCurrentMove(Move currentMove)
    {
        if (_50437079C366407D978Fe4Afd60C535F.CurrentMove == Move.Idling)
        {
            _50437079C366407D978Fe4Afd60C535F.CurrentMove = currentMove;

            return true;
        }

        return false;
    }

    private void SetCurrentMove(Move currentMove, int x, int y)
    {
        if (SetCurrentMove(currentMove))
        {
            _50437079C366407D978Fe4Afd60C535F.MoveDestinationX = x;
            _50437079C366407D978Fe4Afd60C535F.MoveDestinationY = y;
        }
    }

    #endregion
}