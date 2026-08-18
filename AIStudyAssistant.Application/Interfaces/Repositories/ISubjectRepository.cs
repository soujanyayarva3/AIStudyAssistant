using AIStudyAssistant.Domain.Entities;

namespace AIStudyAssistant.Application.Interfaces.Repositories;

public interface ISubjectRepository
{
    Task<List<Subject>> GetSubjectsAsync(int userId);

    Task<Subject?> GetSubjectByIdAsync(int subjectId, int userId);

    Task<Subject> CreateSubjectAsync(Subject subject);

    Task UpdateSubjectAsync(Subject subject);

    Task DeleteSubjectAsync(Subject subject);
}