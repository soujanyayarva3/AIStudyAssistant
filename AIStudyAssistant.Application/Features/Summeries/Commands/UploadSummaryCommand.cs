using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Summaries.Commands;

public class UploadSummaryCommand : IRequest<Summary>
{
    public string Title { get; set; } = "";

    public int SubjectId { get; set; }

    public string SummaryStyle { get; set; } = "";

    public string FileName { get; set; } = "";

    public string FilePath { get; set; } = "";

    public int UserId { get; set; }

    // NEW

    public string OriginalText { get; set; } = "";

    public string SummaryText { get; set; } = "";

    public string Keywords { get; set; } = "";

    public string Questions { get; set; } = "";
}