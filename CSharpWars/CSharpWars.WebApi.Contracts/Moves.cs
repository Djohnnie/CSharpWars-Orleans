namespace CSharpWars.WebApi.Contracts;

public class GetAllMovesRequest
{
    public string ArenaName { get; set; }
}

public class GetAllMovesResponse
{
    public List<Move> Moves { get; set; }
}

public class Move
{
    public string Description { get; init; }
}