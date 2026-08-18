using AIStudyAssistant.Domain.Entities;
using ProgressEntity = AIStudyAssistant.Domain.Entities.Progress;
namespace AIStudyAssistant.Application.Interfaces.Repositories;

public interface IProgressRepository
{
    Task<ProgressEntity?> GetProgressAsync(int userId);

    Task<ProgressEntity> CreateProgressAsync(ProgressEntity progress);

    Task UpdateProgressAsync(ProgressEntity progress);
}