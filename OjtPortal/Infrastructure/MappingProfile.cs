using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.Entities;

namespace OjtPortal.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Student, NewStudentDto>().ReverseMap();
            CreateMap<DegreeProgram, DegreeProgramDto>().ReverseMap();
        }
    }
}
