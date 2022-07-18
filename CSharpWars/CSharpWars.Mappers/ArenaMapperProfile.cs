using AutoMapper;
using CSharpWars.Orleans.Contracts.Arena;
using CSharpWars.WebApi.Contracts;

namespace CSharpWars.Mappers;

public class ArenaMapperProfile : Profile
{
    public ArenaMapperProfile()
    {
        CreateMap<ArenaDto, GetArenaResponse>();
    }
}