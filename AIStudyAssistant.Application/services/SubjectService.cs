using AIStudyAssistant.Application.Interfaces;
using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;

namespace AIStudyAssistant.Application.Services;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjectRepository;

    public SubjectService(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<List<Subject>> GetSubjectsAsync(int userId)
    {
        return await _subjectRepository.GetSubjectsAsync(userId);
    }
}