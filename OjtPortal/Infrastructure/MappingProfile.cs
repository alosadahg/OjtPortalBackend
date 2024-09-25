using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.Entities;

namespace OjtPortal.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreateMap<Entity, Dto>() format for convenience
            CreateMap<Admin, User>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, ExistingUserDto>().ReverseMap();
            CreateMap<User, FullUserDto>().ReverseMap();
            CreateMap<Student, NewStudentDto>().ReverseMap();
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Student, StudentToInstructorOverviewDto>().ReverseMap();
            CreateMap<Student, StudentToMentorOverviewDto>().ReverseMap();
            CreateMap<Company, NewCompanyDto>().ReverseMap();
            CreateMap<Mentor, NewMentorDto>().ReverseMap();
            CreateMap<Mentor, FullMentorDto>().ReverseMap();
            CreateMap<Chair, ChairDto>().ReverseMap();
            CreateMap<Chair, NewTeacherDto>().ReverseMap();
            CreateMap<Teacher, NewTeacherDto>().ReverseMap();
            CreateMap<Teacher, TeacherDto>().ReverseMap();
            CreateMap<Shift, NewShiftDto>().ReverseMap();
            CreateMap<DegreeProgram, DegreeProgramDto>()
            .ForMember(dto => dto.DepartmentCode,
                       entity => entity.MapFrom(src => src.Department.DepartmentCode))
            .ReverseMap();
        }
    }
}
