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
        Task<(FullMentorDto?, ErrorResponseModel?)> GetMentorByIdAsync(int id);
    }

    public class MentorService : IMentorService
    {
        private readonly IMentorRepo _mentorRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ICompanyRepo _companyRepository;
        private readonly IUserRepo _userRepo;

        public MentorService(IMentorRepo mentorRepository, IMapper mapper, IUserService userService, ICompanyRepo companyRepository, IUserRepo userRepo)
        {
            this._mentorRepository = mentorRepository;
            this._mapper = mapper;
            this._userService = userService;
            this._companyRepository = companyRepository;
            this._userRepo = userRepo;
        }

        public async Task<(FullMentorDto?, ErrorResponseModel?)> AddMentorAsync(NewMentorDto newMentorDto)
        {
            var (createdUser, error) = await _userService.CreateUserAsync(newMentorDto, UserType.Mentor);
            if (error != null) return (null, error);
            var mentorEntity = _mapper.Map<Mentor>(newMentorDto);
            mentorEntity.User = createdUser;
            bool isPasswordRandom = false;

            if (!string.IsNullOrEmpty(newMentorDto.Password))
            {
                newMentorDto.Password = _userService.GeneratePassword();
                isPasswordRandom = true;
            }

            mentorEntity.Company = await _companyRepository.AddCompanyAsync(mentorEntity.Company);

            mentorEntity = await _mentorRepository.AddMentorAsync(mentorEntity);
            if (mentorEntity == null) return (null, new(HttpStatusCode.UnprocessableEntity, LoggingTemplate.DuplicateRecordTitle("mentor"), LoggingTemplate.DuplicateRecordDescription("mentor", newMentorDto.Email)));

            return (_mapper.Map<FullMentorDto>(mentorEntity), null);
        }

        public async Task<(FullMentorDto?, ErrorResponseModel?)> GetMentorByIdAsync(int id)
        {
            var existingMentor = await _mentorRepository.GetMentorByIdAsync(id);
            if (existingMentor == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("mentor"), LoggingTemplate.MissingRecordDescription("mentor", $"{id}")));
            return (_mapper.Map<FullMentorDto>(existingMentor), null);
        }
    }
}
