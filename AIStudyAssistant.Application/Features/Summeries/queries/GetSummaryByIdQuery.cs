
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Summaries.Queries;

public class GetSummaryByIdQuery : IRequest<Summary?>
{
    public int SummaryId { get; set; }

    public int UserId { get; set; }
}

