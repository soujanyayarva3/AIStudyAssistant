namespace AIStudyAssistant.Application.Interfaces;

public interface IProgressCalculationService
{
    Task UpdateProgressAsync(int userId);
}