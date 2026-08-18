using AIStudyAssistant.Domain.Entities;

namespace AIStudyAssistant.Application.Interfaces.Repositories;

public interface ISummaryRepository
{
    Task<List<Summary>> GetSummariesAsync(
        int userId
    );

    Task<Summary?> GetSummaryByIdAsync(
        int id,
        int userId
    );

    Task<Summary> CreateSummaryAsync(
        Summary summary
    );

    Task UpdateSummaryAsync(
        Summary summary
    );

    Task DeleteSummaryAsync(
        Summary summary
    );
}