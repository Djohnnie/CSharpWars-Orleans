using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts;
using CSharpWars.Orleans.Contracts.Grains;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace CSharpWars.Orleans.Grains;

public class MovesState
{
    public bool Exists { get; set; }
    public List<MoveDto> Moves { get; set; }
}

public class MovesGrain : GrainBase<IMovesGrain>, IMovesGrain
{
    private readonly IPersistentState<MovesState> _state;

    public MovesGrain(
        [PersistentState("moves", "movesStore")] IPersistentState<MovesState> state,
        ILogger<IMovesGrain> logger) : base(logger)
    {
        _state = state;
    }

    public async Task AddMove(MoveDto move)
    {
        if (!_state.State.Exists)
        {
            _state.State.Moves = new List<MoveDto>();
            _state.State.Exists = true;
        }

        _state.State.Moves.Add(move);

        if (_state.State.Moves.Count > 25)
        {
            _state.State.Moves.RemoveAt(0);
        }

        await _state.WriteStateAsync();
    }

    public Task<List<MoveDto>> GetMoves()
    {
        var result = _state.State.Exists ? _state.State.Moves : new List<MoveDto>();
        return Task.FromResult(result);
    }
}