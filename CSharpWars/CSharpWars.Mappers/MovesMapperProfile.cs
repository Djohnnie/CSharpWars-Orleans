using AutoMapper;
using CSharpWars.Orleans.Contracts;
using CSharpWars.WebApi.Contracts;

namespace CSharpWars.Mappers;

public class MovesMapperProfile : Profile
{
    public MovesMapperProfile()
    {
        CreateMap<List<MoveDto>, GetAllMovesResponse>()
            .ConstructUsing((src, ctx) => new GetAllMovesResponse(ctx.Mapper.Map<List<Move>>(src)));
        CreateMap<MoveDto, Move>();
    }
}