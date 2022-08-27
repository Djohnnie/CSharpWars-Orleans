using AutoMapper;
using CSharpWars.Orleans.Contracts;
using CSharpWars.WebApi.Contracts;

namespace CSharpWars.Mappers;

public class PlayerMapperProfile : Profile
{
    public PlayerMapperProfile()
    {
        CreateMap<PlayerDto, LoginResponse>();
    }
}