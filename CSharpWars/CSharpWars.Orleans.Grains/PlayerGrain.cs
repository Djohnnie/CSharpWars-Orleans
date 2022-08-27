using CSharpWars.Common.Extensions;
using CSharpWars.Common.Helpers;
using CSharpWars.Helpers;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts;
using CSharpWars.Orleans.Contracts.Grains;
using Microsoft.Extensions.Logging;
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

public class PlayerGrain : GrainBase<IPlayerGrain>, IPlayerGrain
{
    private readonly IPasswordHashHelper _passwordHashHelper;
    private readonly IJwtHelper _jwtHelper;
    private readonly IGrainFactoryHelperWithGuidKey<IBotGrain> _botGrainHelper;
    private readonly ILogger<IPlayerGrain> _logger;
    private readonly IPersistentState<PlayerState> _state;

    public PlayerGrain(
        IPasswordHashHelper passwordHashHelper,
        IJwtHelper jwtHelper,
        IGrainFactoryHelperWithGuidKey<IBotGrain> botGrainHelper,
        ILogger<IPlayerGrain> logger,
        [PersistentState("player", "playerStore")] IPersistentState<PlayerState> state) : base(logger)
    {
        _passwordHashHelper = passwordHashHelper;
        _jwtHelper = jwtHelper;
        _botGrainHelper = botGrainHelper;
        _logger = logger;
        _state = state;
    }

    public async Task<PlayerDto> Login(string username, string password)
    {
        if (!_state.State.Exists)
        {
            _logger.AutoLogInformation($"New login for '{username}'");

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
            _logger.AutoLogInformation($"Existing login for '{username}'");

            var (_, hashed) = _passwordHashHelper.CalculateHash(password, _state.State.PasswordSalt);

            if (_state.State.PasswordHash != hashed)
            {
                throw new ArgumentException("Invalid username and password", nameof(password));
            }
        }

        await _state.WriteStateAsync();

        var token = _jwtHelper.GenerateToken(username);
        _logger.AutoLogInformation($"Generated '{token}' for '{username}'");

        return new PlayerDto(username, token);
    }

    public async Task ValidateBotDeploymentLimit()
    {
        if (!_state.State.Exists)
        {
            throw new ArgumentException("Player does not have state yet!");
        }

        _logger.AutoLogInformation($"Validating bot deployment limit for '{_state.State.Username}'");

        if (_state.State.LastDeployment.HasValue && _state.State.LastDeployment >= DateTime.UtcNow.AddSeconds(-1))
        {
            throw new ArgumentException("You are not allowed to create multiple robots in rapid succession!");
        }

        _state.State.LastDeployment = DateTime.UtcNow;
        await _state.WriteStateAsync();
    }

    public async Task BotCreated(Guid botId)
    {
        if (!_state.State.Exists)
        {
            throw new ArgumentException("Player does not have state yet!");
        }

        _logger.AutoLogInformation($"Bot with ID '{botId}' has been created for player '{_state.State.Username}'");

        _state.State.BotIds.Add(botId);
        await _state.WriteStateAsync();
    }

    public async Task DeletePlayer()
    {
        if (_state.State.Exists)
        {
            _logger.AutoLogInformation($"Deleting player '{_state.State.Username}' and all of its bots");

            foreach (var botId in _state.State.BotIds)
            {
                await _botGrainHelper.FromGrain(botId, g => g.DeleteBot(false));
            }

            await _state.ClearStateAsync();
        }

        _logger.AutoLogInformation($"{nameof(PlayerGrain)} will be deactivated...");
        DeactivateOnIdle();
    }
}