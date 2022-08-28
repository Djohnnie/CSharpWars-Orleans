using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts;
using CSharpWars.Orleans.Contracts.Grains;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace CSharpWars.Orleans.Grains;

public class MessagesState
{
    public bool Exists { get; set; }
    public List<MessageDto> Messages { get; set; }
}

public class MessagesGrain : GrainBase<IMessagesGrain>, IMessagesGrain
{
    private readonly IPersistentState<MessagesState> _state;

    public MessagesGrain(
        [PersistentState("messages", "messagesStore")] IPersistentState<MessagesState> state,
        ILogger<IMessagesGrain> logger) : base(logger)
    {
        _state = state;
    }

    public async Task AddMessage(MessageDto message)
    {
        if (!_state.State.Exists)
        {
            _state.State.Messages = new List<MessageDto>();
            _state.State.Exists = true;
        }

        _state.State.Messages.Add(message);

        if (_state.State.Messages.Count > 25)
        {
            _state.State.Messages.RemoveAt(0);
        }

        await _state.WriteStateAsync();
    }

    public Task<List<MessageDto>> GetMessages()
    {
        var result = _state.State.Exists ? _state.State.Messages : new List<MessageDto>();
        return Task.FromResult(result);
    }
}