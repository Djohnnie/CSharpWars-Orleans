using CSharpWars.Common.Extensions;
using CSharpWars.Common.Helpers;
using CSharpWars.Enums;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts;
using CSharpWars.Orleans.Contracts.Grains;
using CSharpWars.Scripting;
using CSharpWars.Scripting.Moves;

namespace CSharpWars.Orleans.Grains.Logic;

public interface IPostprocessingLogic
{
    Task Go(ProcessingContext context);
}

public class PostprocessingLogic : IPostprocessingLogic
{
    private static readonly string[] IdleMessages = {
        "Is idling",
        "Is too lazy to do anything",
        "Wants to be a sloth",
        "Is goofing off",
        "Is a die-hard couch-potato",
        "Is wasting time looking at the sky"
    };

    private readonly IRandomHelper _randomHelper;
    private readonly IGrainFactoryHelperWithStringKey<IMovesGrain> _movesGrainHelper;
    private readonly IGrainFactoryHelperWithStringKey<IMessagesGrain> _messagesGrainHelper;

    public PostprocessingLogic(
        IRandomHelper randomHelper,
        IGrainFactoryHelperWithStringKey<IMovesGrain> movesGrainHelper,
        IGrainFactoryHelperWithStringKey<IMessagesGrain> messagesGrainHelper)
    {
        _randomHelper = randomHelper;
        _movesGrainHelper = movesGrainHelper;
        _messagesGrainHelper = messagesGrainHelper;
    }

    public async Task Go(ProcessingContext context)
    {
        var botProperties = context.GetOrderedBotProperties();
        var botLog = new List<BotLog>();

        foreach (var botProperty in botProperties)
        {
            var bot = context.Bots.Single(x => x.BotId == botProperty.BotId);
            var botResult = BaseMove.Build(botProperty, _randomHelper).Go();
            bot.Orientation = botResult.Orientation;
            bot.FromX = bot.X;
            bot.FromY = bot.Y;
            bot.X = botResult.X;
            bot.Y = botResult.Y;
            bot.CurrentHealth = botResult.CurrentHealth;
            bot.CurrentStamina = botResult.CurrentStamina;
            bot.Move = botResult.Move;
            bot.Memory = botResult.Memory.Serialize();
            bot.LastAttackX = botResult.LastAttackX;
            bot.LastAttackY = botResult.LastAttackY;

            context.UpdateBotProperties(bot);

            foreach (var otherBot in context.Bots.Where(x => x.BotId != bot.BotId))
            {
                otherBot.CurrentHealth -= botResult.GetInflictedDamage(otherBot.BotId);
                var teleportation = botResult.GetTeleportation(otherBot.BotId);
                if (teleportation != (-1, -1))
                {
                    otherBot.X = teleportation.X;
                    otherBot.Y = teleportation.Y;
                }

                var otherBotProperties = botProperties.Single(x => x.BotId == otherBot.BotId);
                otherBotProperties.Update(otherBot);
            }

            await SendMessage(context.Arena.Name, botProperty.Name, botProperty.PlayerName, botResult.Message);
            await LogMove(context.Arena.Name, bot.BotName, bot.PlayerName, botResult.Move);
        }

        foreach (var bot in context.Bots)
        {
            if (bot.CurrentHealth <= 0)
            {
                bot.CurrentHealth = 0;
                bot.TimeOfDeath = DateTime.UtcNow;
                if (bot.Move != Move.SelfDestruct)
                {
                    bot.Move = Move.Died;
                }
            }

            if (bot.CurrentStamina <= 0)
            {
                bot.CurrentStamina = 0;
            }
        }
    }

    private async Task LogMove(string arenaName, string botName, string playerName, Move move)
    {
        var moveDetails = new MoveDto
        {
            TimeStamp = DateTime.UtcNow,
            Owner = $"{botName} ({playerName})",
            Move = move,
            Description = BuildMoveDescription(move, 0, 0),
            Target = ""
        };

        await _movesGrainHelper.FromGrain(arenaName, g => g.AddMove(moveDetails));
    }

    private async Task SendMessage(string arenaName, string botName, string playerName, string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            var messageDetails = new MessageDto
            {
                TimeStamp = DateTime.UtcNow,
                Owner = $"{botName} ({playerName})",
                Message = message
            };

            await _messagesGrainHelper.FromGrain(arenaName, g => g.AddMessage(messageDetails));
        }
    }

    private string BuildMoveDescription(Move move, int targetX, int targetY)
    {
        return move switch
        {
            Move.Idling => _randomHelper.GetItem(IdleMessages),
            Move.TurningLeft => "Is turning left",
            Move.TurningRight => "Is turning right",
            Move.TurningAround => "Is turning around",
            Move.WalkForward => "Is walking forward",
            Move.Teleport => $"Is teleporting towards ({targetX},{targetY})",
            Move.MeleeAttack => "Is attacking",
            Move.RangedAttack => $"Is attacking at range towards ({targetX},{targetY})",
            Move.SelfDestruct => "Has self destructed because the world is a harsh place",
            Move.Died => "Has died",
            Move.ScriptError or _ => "Is confused by a simple error in judgment"
        };
    }
}