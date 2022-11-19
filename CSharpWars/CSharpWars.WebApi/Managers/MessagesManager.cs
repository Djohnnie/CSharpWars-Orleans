using AutoMapper;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts.Grains;
using CSharpWars.WebApi.Contracts;

namespace CSharpWars.WebApi.Managers;

public interface IMessagesManager
{
    Task<GetAllMessagesResponse> GetAllMessages(GetAllMessagesRequest request);
}

public class MessagesManager : IMessagesManager
{
    private readonly IGrainFactoryHelperWithStringKey<IMessagesGrain> _messagesGrainClient;
    private readonly IMapper _mapper;

    public MessagesManager(
        IGrainFactoryHelperWithStringKey<IMessagesGrain> messagesGrainClient,
        IMapper mapper)
    {
        _messagesGrainClient = messagesGrainClient;
        _mapper = mapper;
    }

    public async Task<GetAllMessagesResponse> GetAllMessages(GetAllMessagesRequest request)
    {
        var messages = await _messagesGrainClient.FromGrain(request.ArenaName, g => g.GetMessages());
        return _mapper.Map<GetAllMessagesResponse>(messages);
    }
}