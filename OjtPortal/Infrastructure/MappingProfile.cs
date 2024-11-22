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
            CreateMap<Student, NewStudentDto>().ReverseMap();
            CreateMap<Student, MentorAddStudentDto>().ReverseMap();
            CreateMap<Student, TeacherAddStudentDto>().ReverseMap();
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Student, StudentToInstructorOverviewDto>().ReverseMap();
            CreateMap<Student, StudentToMentorOverviewDto>().ReverseMap();
            CreateMap<Student, UpdateStudentDto>().ReverseMap();
            CreateMap<Student, StudentPerformance>().ReverseMap();
            CreateMap<Student, TrainingPlanRequestDto>().ReverseMap();
            CreateMap<Company, NewCompanyDto>().ReverseMap();
            CreateMap<Mentor, NewMentorDto>().ReverseMap();
            CreateMap<Mentor, FullMentorDto>().ReverseMap();
            CreateMap<Mentor, FullMentorDtoWithStudents>().ReverseMap();
            CreateMap<Mentor, FullMentorDtoWithSubMentors>().ReverseMap();
            CreateMap<Mentor, MentorDto>().ReverseMap();
            CreateMap<Chair, ChairDto>().ReverseMap();
            CreateMap<Chair, NewTeacherDto>().ReverseMap();
            CreateMap<Teacher, NewTeacherDto>().ReverseMap();
            CreateMap<Teacher, TeacherDto>().ReverseMap();
            CreateMap<Teacher, TeacherNoStudentsDto>().ReverseMap();
            CreateMap<Teacher, TeacherDtoWithStudents>().ReverseMap();
            CreateMap<Shift, NewShiftDto>().ReverseMap();
            CreateMap<DegreeProgram, DegreeProgramDto>()
            .ForMember(dto => dto.DepartmentCode,
                       entity => entity.MapFrom(src => src.Department.DepartmentCode))
            .ReverseMap();
            CreateMap<NewStudentDto, MentorAddStudentDto>().ReverseMap();
            CreateMap<NewStudentDto, TeacherAddStudentDto>().ReverseMap();
            CreateMap<Attendance, AttendanceDto>().ReverseMap();
            CreateMap<LogbookEntry, NewLogbookEntryDto>().ReverseMap();
            CreateMap<LogbookEntry, LogbookDto>().ReverseMap();
            CreateMap<TrainingPlan, NewTrainingPlanDto>().ReverseMap();
            CreateMap<TrainingPlan, TrainingPlanFromApiDto>().ReverseMap();
            CreateMap<TrainingPlan, TrainingPlanDto>().ReverseMap();
            CreateMap<TrainingPlan, TrainingPlanRequestDto>().ReverseMap();
            CreateMap<StudentTraining, AssignTrainingPlanDto>().ReverseMap();
            CreateMap<StudentTraining, AssignedTrainingPlanToStudentDto>().ReverseMap();
            CreateMap<StudentTask, AssignedTaskDto>().ReverseMap();
            CreateMap<TrainingTask, NewTaskDto>().ReverseMap();
            CreateMap<TrainingTask, TaskDto>().ReverseMap();
            CreateMap<TrainingTask, TaskWithStackAndSkillDto>().ReverseMap();
            CreateMap<Skill, NewSkillDto>().ReverseMap();
            CreateMap<TechStack, NewTechStackDto>().ReverseMap();
            CreateMap<StudentTask, StudentTaskDto>().ReverseMap();
            CreateMap<SubMentor, SubMentorDto>().ReverseMap();
            CreateMap<SubMentor, Mentor>().ReverseMap();
            CreateMap<SubMentor, MentorDto>().ReverseMap();
            CreateMap<SubMentor, FullMentorDtoWithStudents>().ReverseMap();
        }
    }
}
