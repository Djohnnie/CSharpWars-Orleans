using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts;
using CSharpWars.Orleans.Contracts.Grains;

namespace CSharpWars.Web.Client;

public interface IOrleansClient
{
    Task<PlayerDto> Login(string username, string password);
    Task<ValidatedScriptDto> Validate(ScriptToValidateDto scriptToValidate);
    Task<BotDto> CreateBot(string playerName, BotToCreateDto bot);
}

public class OrleansClient : IOrleansClient
{
    private readonly IClusterClientHelper<IPlayersGrain> _playersGrainClient;
    private readonly IClusterClientHelperWithGuidKey<IValidationGrain> _validationGrainClient;
    private readonly IClusterClientHelperWithStringKey<IArenaGrain> _arenaGrainClient;

    public OrleansClient(
        IClusterClientHelper<IPlayersGrain> playersGrainClient,
        IClusterClientHelperWithGuidKey<IValidationGrain> validationGrainClient,
        IClusterClientHelperWithStringKey<IArenaGrain> arenaGrainClient)
    {
        _playersGrainClient = playersGrainClient;
        _validationGrainClient = validationGrainClient;
        _arenaGrainClient = arenaGrainClient;
    }

    public async Task<PlayerDto> Login(string username, string password)
    {
        return await _playersGrainClient.FromGrain(g => g.Login(username, password));
    }

    public async Task<ValidatedScriptDto> Validate(ScriptToValidateDto scriptToValidate)
    {
        return await _validationGrainClient.FromGrain(Guid.NewGuid(), g => g.Validate(scriptToValidate));
    }

    public async Task<BotDto> CreateBot(string playerName, BotToCreateDto bot)
    {
        return await _arenaGrainClient.FromGrain(bot.ArenaName, g => g.CreateBot(playerName, bot));
    }
}