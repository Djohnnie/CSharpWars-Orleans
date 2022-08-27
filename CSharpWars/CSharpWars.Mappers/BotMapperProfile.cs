using AutoMapper;
using CSharpWars.Orleans.Contracts;
using CSharpWars.WebApi.Contracts;

namespace CSharpWars.Mappers;

public class BotMapperProfile : Profile
{
    public BotMapperProfile()
    {
        CreateMap<CreateBotRequest, BotToCreateDto>();
        CreateMap<BotDto, CreateBotResponse>();
        CreateMap<List<BotDto>, GetAllActiveBotsResponse>()
            .ConstructUsing((src, ctx) => new GetAllActiveBotsResponse(ctx.Mapper.Map<List<Bot>>(src)));
        CreateMap<BotDto, Bot>();
    }
}