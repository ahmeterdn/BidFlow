using AutoMapper;
using BidFlow.Api.DTOs;
using BidFlow.Entities;

namespace BidFlow.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, AppUserDto>().ReverseMap();
            
        }
    }
}
