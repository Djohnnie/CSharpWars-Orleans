using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts;
using CSharpWars.Orleans.Contracts.Grains;

namespace CSharpWars.Web.Client;

public interface IOrleansClient
{
    Task<PlayerDto> Login(string username, string password);
    Task<ValidatedScriptDto> Validate(ScriptToValidateDto scriptToValidate);
    Task<BotDto> CreateBot(string playerName, BotToCreateDto bot);
    Task<List<MoveDto>> GetMoves(string arenaName);
    Task<List<MessageDto>> GetMessages(string arenaName);
}

public class OrleansClient : IOrleansClient
{
    private readonly IClusterClientHelper<IPlayersGrain> _playersGrainClient;
    private readonly IClusterClientHelperWithGuidKey<IValidationGrain> _validationGrainClient;
    private readonly IClusterClientHelperWithStringKey<IArenaGrain> _arenaGrainClient;
    private readonly IClusterClientHelperWithStringKey<IMovesGrain> _movesGrainClient;
    private readonly IClusterClientHelperWithStringKey<IMessagesGrain> _messagesGrainClient;

    public OrleansClient(
        IClusterClientHelper<IPlayersGrain> playersGrainClient,
        IClusterClientHelperWithGuidKey<IValidationGrain> validationGrainClient,
        IClusterClientHelperWithStringKey<IArenaGrain> arenaGrainClient,
        IClusterClientHelperWithStringKey<IMovesGrain> movesGrainClient,
        IClusterClientHelperWithStringKey<IMessagesGrain> messagesGrainClient)
    {
        _playersGrainClient = playersGrainClient;
        _validationGrainClient = validationGrainClient;
        _arenaGrainClient = arenaGrainClient;
        _movesGrainClient = movesGrainClient;
        _messagesGrainClient = messagesGrainClient;
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

    public async Task<List<MoveDto>> GetMoves(string arenaName)
    {
        return await _movesGrainClient.FromGrain(arenaName, g => g.GetMoves());
    }

    public async Task<List<MessageDto>> GetMessages(string arenaName)
    {
        return await _messagesGrainClient.FromGrain(arenaName, g => g.GetMessages());
    }
}