using AIStudyAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace AIStudyAssistant.Application.Features.Progress.Commands;

public class UpdateProgressCommandHandler : IRequestHandler<UpdateProgressCommand>
{
    private readonly IProgressRepository _repository;

    public UpdateProgressCommandHandler(IProgressRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
        UpdateProgressCommand request,
        CancellationToken cancellationToken)
    {
        var progress = await _repository.GetProgressAsync(request.UserId);

        if (progress == null)
        {
            progress = new Domain.Entities.Progress
            {
                UserId = request.UserId
            };

            await _repository.CreateProgressAsync(progress);
        }

        progress.TotalSubjects = request.TotalSubjects;
        progress.TotalNotes = request.TotalNotes;
        progress.CompletedStudyPlans = request.CompletedStudyPlans;
        progress.TotalStudyPlans = request.TotalStudyPlans;
        progress.TotalQuizzes = request.TotalQuizzes;
        progress.AverageQuizScore = request.AverageQuizScore;
        progress.ProgressPercentage = request.ProgressPercentage;
        progress.LastUpdated = DateTime.UtcNow;

        await _repository.UpdateProgressAsync(progress);

        return Unit.Value;
    }
}