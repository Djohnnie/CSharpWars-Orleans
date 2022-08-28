using Orleans;

namespace CSharpWars.Orleans.Contracts.Grains;

public interface IMovesGrain : IGrainWithStringKey
{
    Task AddMove(MoveDto move);

    Task<List<MoveDto>> GetMoves();
}