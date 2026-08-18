using AIStudyAssistant.Domain.Entities;

namespace AIStudyAssistant.Application.Interfaces;

public interface ISubjectService
{
    Task<List<Subject>> GetSubjectsAsync(int userId);
}