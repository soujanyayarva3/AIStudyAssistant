
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Summaries.Commands;

public class CreateSummaryCommand : IRequest<Summary>
{
    public string Title { get; set; } = string.Empty;

    public int SubjectId { get; set; }

    public string SummaryStyle { get; set; } = string.Empty;

    public string OriginalText { get; set; } = string.Empty;

    public string SummaryText { get; set; } = string.Empty;

    public int UserId { get; set; }
}

