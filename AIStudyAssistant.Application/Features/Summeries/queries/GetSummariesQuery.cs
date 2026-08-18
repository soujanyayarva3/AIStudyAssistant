
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Summaries.Queries;

public record GetSummariesQuery(int UserId)
    : IRequest<List<Summary>>;

