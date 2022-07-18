using AutoMapper;
using CSharpWars.Orleans.Contracts.Bot;
using CSharpWars.WebApi.Contracts;

namespace CSharpWars.Mappers;

public class BotMapperProfile : Profile
{
    public BotMapperProfile()
    {
        CreateMap<BotDto, CreateBotResponse>();
        CreateMap<List<BotDto>, GetAllActiveBotsResponse>();
    }
}