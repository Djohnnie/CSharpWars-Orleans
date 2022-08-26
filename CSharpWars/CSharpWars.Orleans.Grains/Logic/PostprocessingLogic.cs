using CSharpWars.Common.Extensions;
using CSharpWars.Common.Helpers;
using CSharpWars.Enums;
using CSharpWars.Scripting;
using CSharpWars.Scripting.Moves;

namespace CSharpWars.Orleans.Grains.Logic;

public interface IPostprocessingLogic
{
    Task Go(ProcessingContext context);
}

public class PostprocessingLogic : IPostprocessingLogic
{
    private readonly IRandomHelper _randomHelper;

    public PostprocessingLogic(IRandomHelper randomHelper)
    {
        _randomHelper = randomHelper;
    }

    public Task Go(ProcessingContext context)
    {
        var botProperties = context.GetOrderedBotProperties();

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

        return Task.CompletedTask;
    }
}