namespace CSharpWars.WebApi.Contracts;

public record GetAllMovesRequest(string ArenaName);

public record GetAllMovesResponse(List<Move> Moves);

public record Move
{
    public string Description { get; init; }
}