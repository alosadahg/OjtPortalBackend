using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.Entities;

namespace OjtPortal.Infrastructure
{
    public class MappingProfile : Profile
    {
        protected MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
