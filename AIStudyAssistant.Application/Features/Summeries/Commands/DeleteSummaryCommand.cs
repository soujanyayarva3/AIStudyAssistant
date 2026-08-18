using MediatR;

namespace AIStudyAssistant.Application.Features.Summaries.Commands;

public class DeleteSummaryCommand : IRequest
{
    public int SummaryId { get; set; }

    public int UserId { get; set; }
}