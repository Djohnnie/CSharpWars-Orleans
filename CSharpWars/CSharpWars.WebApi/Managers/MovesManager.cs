using AutoMapper;
using CSharpWars.Orleans.Common;
using CSharpWars.Orleans.Contracts.Grains;
using CSharpWars.WebApi.Contracts;

namespace CSharpWars.WebApi.Managers;

public interface IMovesManager
{
    Task<GetAllMovesResponse> GetMoves(GetAllMovesRequest request);
}

public class MovesManager : IMovesManager
{
    private readonly IClusterClientHelperWithStringKey<IMovesGrain> _movesGrainClient;
    private readonly IMapper _mapper;

    public MovesManager(
        IClusterClientHelperWithStringKey<IMovesGrain> movesGrainClient,
        IMapper mapper)
    {
        _movesGrainClient = movesGrainClient;
        _mapper = mapper;
    }

    public async Task<GetAllMovesResponse> GetMoves(GetAllMovesRequest request)
    {
        var messages = await _movesGrainClient.FromGrain(request.ArenaName, g => g.GetMoves());
        return _mapper.Map<GetAllMovesResponse>(messages);
    }
}