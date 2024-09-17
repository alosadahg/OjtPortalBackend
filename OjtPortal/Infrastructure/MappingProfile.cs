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
            CreateMap<User, ExistingUserDto>().ReverseMap();
            CreateMap<Student, NewStudentDto>().ReverseMap();
            CreateMap<DegreeProgram, DegreeProgramDto>().ReverseMap();
            CreateMap<Company, NewCompanyDto>().ReverseMap();
            CreateMap<Mentor, NewMentorDto>().ReverseMap();
            CreateMap<Mentor, FullMentorDto>().ReverseMap();
            CreateMap<Teacher, NewTeacherDto>().ReverseMap();
            CreateMap<Teacher, TeacherDto>().ReverseMap();
        }
    }
}
