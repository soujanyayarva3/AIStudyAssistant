using MediatR;

namespace AIStudyAssistant.Application.Features.Summaries.Commands;

public class UpdateSummaryCommand : IRequest
{
    public int SummaryId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string OriginalText { get; set; } = string.Empty;

    public string SummaryText { get; set; } = string.Empty;
}