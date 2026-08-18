using AIStudyAssistant.Domain.Entities;

namespace AIStudyAssistant.Application.Interfaces.Repositories;

public interface IStudyPlanRepository
{
    Task<List<StudyPlan>> GetStudyPlansAsync(int userId);

    Task<StudyPlan?> GetStudyPlanByIdAsync(int planId, int userId);

    Task<StudyPlan> CreateStudyPlanAsync(StudyPlan studyPlan);

    Task UpdateStudyPlanAsync(StudyPlan studyPlan);

    Task DeleteStudyPlanAsync(StudyPlan studyPlan);
}