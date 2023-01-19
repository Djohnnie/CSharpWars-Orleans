namespace CSharpWars.Orleans.Contracts.Grains;

public interface IMessagesGrain : IGrainWithStringKey
{
    Task AddMessage(MessageDto message);

    Task<List<MessageDto>> GetMessages();
}