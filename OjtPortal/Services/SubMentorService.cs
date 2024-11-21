using System.Net;
using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;

namespace OjtPortal.Services
{
    public interface ISubMentorService
    {
        Task<(SubMentorDto?, ErrorResponseModel?)> RegisterSubmentor(int mentorId, int submentorId);
    }

    public class SubMentorService : ISubMentorService
    {
        private readonly IMentorRepo _mentorRepo;
        private readonly ISubMentorRepo _subMentorRepo;
        private readonly IMapper _mapper;

        public SubMentorService(IMentorRepo mentorRepo, ISubMentorRepo subMentorRepo, IMapper mapper)
        {
            this._mentorRepo = mentorRepo;
            this._subMentorRepo = subMentorRepo;
            this._mapper = mapper;
        }
        public async Task<(SubMentorDto?, ErrorResponseModel?)> RegisterSubmentor(int mentorId, int submentorId)
        {
            var existingMentor = await _mentorRepo.GetMentorByIdAsync(mentorId, false, false, true);
            if (existingMentor == null) return (null, new ErrorResponseModel(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("mentor"), LoggingTemplate.MissingRecordDescription("mentor", mentorId.ToString())));
            if (mentorId == submentorId) return (null, new ErrorResponseModel(HttpStatusCode.UnprocessableEntity, "Invalid ID", "Cannot delegate submentor to itself"));
            var existingSubmentor = await _mentorRepo.GetMentorByIdAsync(submentorId, false, false, true);
            if (existingSubmentor == null) return (null, new ErrorResponseModel(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("submentor"), LoggingTemplate.MissingRecordDescription("submentor", submentorId.ToString())));

            if (existingMentor.CompanyId != existingSubmentor.CompanyId)
                return (null, new ErrorResponseModel(HttpStatusCode.MethodNotAllowed, "Invalid Company", "Submentor and mentor have different companies"));
            
            var newSubmentor = new SubMentor
            {
                SubmentorId = existingSubmentor.UserId,
                Submentor = existingSubmentor,
                HeadMentor = existingMentor
            };
            newSubmentor = await _subMentorRepo.AddSubMentorAsync(newSubmentor);
            if (newSubmentor == null) return (null, new ErrorResponseModel(HttpStatusCode.UnprocessableContent, "Submentor creation failed", "Please check logs"));
            return (_mapper.Map<SubMentorDto>(newSubmentor), null);
        }
    }
}
