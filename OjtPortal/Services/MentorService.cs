using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using System.Net;

namespace OjtPortal.Services
{
    public interface IMentorService
    {
        Task<(FullMentorDto?, ErrorResponseModel?)> AddMentorAsync(NewMentorDto newMentorDto);
        Task<(FullMentorDto?, ErrorResponseModel?)> GetMentorByIdAsync(int id, bool includeUser);
    }

    public class MentorService : IMentorService
    {
        private readonly IMentorRepo _mentorRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ICompanyRepo _companyRepository;
        private readonly IUserRepo _userRepo;
        private readonly IStudentService _studentService;

        public MentorService(IMentorRepo mentorRepository, IMapper mapper, IUserService userService, ICompanyRepo companyRepository, IUserRepo userRepo, IStudentService studentService)
        {
            this._mentorRepository = mentorRepository;
            this._mapper = mapper;
            this._userService = userService;
            this._companyRepository = companyRepository;
            this._userRepo = userRepo;
            this._studentService = studentService;
        }

        public async Task<(FullMentorDto?, ErrorResponseModel?)> AddMentorAsync(NewMentorDto newMentorDto)
        {
            var (createdUser, error) = await _userService.CreateUserAsync(newMentorDto, newMentorDto.Password, UserType.Mentor);
            if (error != null) return (null, error);
            var mentorEntity = _mapper.Map<Mentor>(newMentorDto);
            mentorEntity.User = createdUser!.User;
            if (createdUser!.IsPasswordGenerated)
            {
                newMentorDto.Password = createdUser.Password;
                var emailError = _userService.SendActivationEmailAsync(newMentorDto.Email, createdUser.User!, newMentorDto.Password);
                if (emailError.Result != null) return (null, emailError.Result);
            }
            else
            {
                var emailError = _userService.SendActivationEmailAsync(newMentorDto.Email, createdUser.User!);
                if (emailError.Result != null) return (null, emailError.Result);
            }

            mentorEntity.Company = await _companyRepository.AddCompanyAsync(mentorEntity.Company);

            mentorEntity = await _mentorRepository.AddMentorAsync(mentorEntity);
            if (mentorEntity == null) return (null, new(HttpStatusCode.UnprocessableEntity, LoggingTemplate.DuplicateRecordTitle("mentor"), LoggingTemplate.DuplicateRecordDescription("mentor", newMentorDto.Email)));

            return (_mapper.Map<FullMentorDto>(mentorEntity), null);
        }

        public async Task<(FullMentorDto?, ErrorResponseModel?)> GetMentorByIdAsync(int id, bool includeUser)
        {
            var existingMentor = await _mentorRepository.GetMentorByIdAsync(id, includeUser);
            if (existingMentor == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("mentor"), LoggingTemplate.MissingRecordDescription("mentor", $"{id}")));
            var mentorDto = _mapper.Map<FullMentorDto>(existingMentor);
            if (existingMentor.Students != null)
            {
                mentorDto.Interns = _studentService.MapStudentsToDtoList<StudentToMentorOverviewDto>(existingMentor.Students!);
                mentorDto.InternCount = mentorDto.Interns.Count();
            }
            return (mentorDto, null);
        }
    }
}
