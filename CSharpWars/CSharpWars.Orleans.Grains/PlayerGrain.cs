using CSharpWars.Common.Helpers;
using CSharpWars.Helpers;
using CSharpWars.Orleans.Contracts.Player;
using Orleans;
using Orleans.Runtime;

namespace CSharpWars.Orleans.Grains;


public class PlayerState
{
    public bool Exists { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public DateTime? LastDeployment { get; set; }
    public List<Guid> BotIds { get; set; }
}

public interface IPlayerGrain : IGrainWithStringKey
{
    Task<PlayerDto> Login(string username, string password);
    Task ValidateBotDeploymentLimit();
    Task BotCeated(Guid botId);
    Task DeletePlayer();
}

public class PlayerGrain : Grain, IPlayerGrain
{
    private readonly IPasswordHashHelper _passwordHashHelper;
    private readonly IJwtHelper _jwtHelper;
    private readonly IPersistentState<PlayerState> _state;

    public PlayerGrain(
        IPasswordHashHelper passwordHashHelper,
        IJwtHelper jwtHelper,
        [PersistentState("player", "playerStore")] IPersistentState<PlayerState> state)
    {
        _passwordHashHelper = passwordHashHelper;
        _jwtHelper = jwtHelper;
        _state = state;
    }

    public async Task<PlayerDto> Login(string username, string password)
    {
        if (!_state.State.Exists)
        {
            var (salt, hashed) = _passwordHashHelper.CalculateHash(password);

            _state.State.Exists = true;
            _state.State.Username = username;
            _state.State.PasswordSalt = salt;
            _state.State.PasswordHash = hashed;
            _state.State.LastDeployment = null;
            _state.State.BotIds = new List<Guid>();
        }
        else
        {
            var (_, hashed) = _passwordHashHelper.CalculateHash(password, _state.State.PasswordSalt);

            if (_state.State.PasswordHash != hashed)
            {
                throw new ArgumentException("Invalid username and password", nameof(password));
            }
        }

        await _state.WriteStateAsync();

        var token = _jwtHelper.GenerateToken(username);
        return new PlayerDto(username, token);
    }

    public async Task ValidateBotDeploymentLimit()
    {
        if (!_state.State.Exists)
        {
            throw new ArgumentException("Player does not have state yet!");
        }

        if (_state.State.LastDeployment.HasValue && _state.State.LastDeployment >= DateTime.UtcNow.AddSeconds(-1))
        {
            throw new ArgumentException("You are not allowed to create multiple robots in rapid succession!");
        }

        _state.State.LastDeployment = DateTime.UtcNow;
        await _state.WriteStateAsync();
    }

    public async Task BotCeated(Guid botId)
    {
        if (!_state.State.Exists)
        {
            throw new ArgumentException("Player does not have state yet!");
        }

        _state.State.BotIds.Add(botId);

        await _state.WriteStateAsync();
    }

    public async Task DeletePlayer()
    {
        if (_state.State.Exists)
        {
            foreach (var botId in _state.State.BotIds)
            {
                var botGrain = GrainFactory.GetGrain<IBotGrain>(botId);
                await botGrain.DeleteBot();
            }

            await _state.ClearStateAsync();
        }

        DeactivateOnIdle();
    }
}