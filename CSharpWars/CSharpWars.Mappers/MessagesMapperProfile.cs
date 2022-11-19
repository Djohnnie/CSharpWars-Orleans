using AutoMapper;
using CSharpWars.Orleans.Contracts;
using CSharpWars.WebApi.Contracts;

namespace CSharpWars.Mappers;

public class MessagesMapperProfile : Profile
{
    public MessagesMapperProfile()
    {
        CreateMap<List<MessageDto>, GetAllMessagesResponse>()
            .ConstructUsing((src, ctx) => new GetAllMessagesResponse { Messages = ctx.Mapper.Map<List<Message>>(src) });
        CreateMap<MessageDto, Message>()
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Message));
    }
}