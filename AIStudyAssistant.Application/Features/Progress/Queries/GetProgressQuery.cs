using MediatR;
using ProgressEntity = AIStudyAssistant.Domain.Entities.Progress;

namespace AIStudyAssistant.Application.Features.Progress.Queries;

public record GetProgressQuery(int UserId) : IRequest<ProgressEntity?>;