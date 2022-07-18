using AutoMapper;
using CSharpWars.Orleans.Contracts.Player;
using CSharpWars.WebApi.Contracts;

namespace CSharpWars.Mappers;

public class PlayerMapperProfile : Profile
{
    public PlayerMapperProfile()
    {
        CreateMap<PlayerDto, LoginResponse>();
    }
}