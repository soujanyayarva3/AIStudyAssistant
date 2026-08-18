using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Summaries.Commands;

public class UploadSummaryCommandHandler
    : IRequestHandler<UploadSummaryCommand, Summary>
{
    private readonly ISummaryRepository _repository;

    public UploadSummaryCommandHandler(ISummaryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Summary> Handle(
        UploadSummaryCommand request,
        CancellationToken cancellationToken)
    {
        var summary = new Summary
        {
            Title = request.Title,
            SubjectId = request.SubjectId,
            SummaryStyle = request.SummaryStyle,

            FileName = request.FileName,
            FilePath = request.FilePath,

            UserId = request.UserId,
            CreatedDate = DateTime.UtcNow,

            // NEW
            OriginalText = request.OriginalText,
            SummaryText = request.SummaryText,
            Keywords = request.Keywords,
            Questions = request.Questions,

            IsGenerated = true
        };

        return await _repository.CreateSummaryAsync(summary);
    }
}