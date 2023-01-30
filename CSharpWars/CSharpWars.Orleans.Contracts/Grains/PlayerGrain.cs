namespace CSharpWars.Orleans.Contracts.Grains;

public interface IPlayerGrain : IGrainWithStringKey
{
    Task<PlayerDto> Login(string username, string password);
    Task ValidateBotDeploymentLimit();
    Task BotCreated(Guid botId);
    Task DeletePlayer();
}