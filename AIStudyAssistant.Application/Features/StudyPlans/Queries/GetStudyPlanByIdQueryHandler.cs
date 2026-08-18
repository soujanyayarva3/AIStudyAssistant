using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.StudyPlans.Queries;

public class GetStudyPlanByIdQueryHandler
    : IRequestHandler<GetStudyPlanByIdQuery, StudyPlan?>
{
    private readonly IStudyPlanRepository _repository;

    public GetStudyPlanByIdQueryHandler(IStudyPlanRepository repository)
    {
        _repository = repository;
    }

    public async Task<StudyPlan?> Handle(
        GetStudyPlanByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetStudyPlanByIdAsync(
            request.PlanId,
            request.UserId);
    }
}