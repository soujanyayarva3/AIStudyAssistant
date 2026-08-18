using AIStudyAssistant.Application.Interfaces.Repositories;
using ProgressEntity = AIStudyAssistant.Domain.Entities.Progress;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Progress.Queries;

public class GetProgressQueryHandler : IRequestHandler<GetProgressQuery, ProgressEntity?>
{
    private readonly IProgressRepository _repository;

    public GetProgressQueryHandler(IProgressRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProgressEntity?> Handle(
        GetProgressQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetProgressAsync(request.UserId);
    }
}