using AIStudyAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace AIStudyAssistant.Application.Features.Summaries.Commands;

public class UpdateSummaryCommandHandler : IRequestHandler<UpdateSummaryCommand>
{
    private readonly ISummaryRepository _repository;

    public UpdateSummaryCommandHandler(ISummaryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateSummaryCommand request, CancellationToken cancellationToken)
    {
        var summary = await _repository.GetSummaryByIdAsync(request.SummaryId, request.UserId);

        if (summary == null)
            return Unit.Value;

        summary.Title = request.Title;
        summary.OriginalText = request.OriginalText;
        summary.SummaryText = request.SummaryText;

        await _repository.UpdateSummaryAsync(summary);

        return Unit.Value;
    }
}