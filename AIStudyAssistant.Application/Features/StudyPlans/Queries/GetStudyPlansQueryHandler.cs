using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.StudyPlans.Queries;

public class GetStudyPlansQueryHandler
    : IRequestHandler<GetStudyPlansQuery, List<StudyPlan>>
{
    private readonly IStudyPlanRepository _repository;

    public GetStudyPlansQueryHandler(IStudyPlanRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<StudyPlan>> Handle(
        GetStudyPlansQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetStudyPlansAsync(request.UserId);
    }
}