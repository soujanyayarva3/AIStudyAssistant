using AIStudyAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace AIStudyAssistant.Application.Features.StudyPlans.Commands;

public class DeleteStudyPlanCommandHandler
    : IRequestHandler<DeleteStudyPlanCommand>
{
    private readonly IStudyPlanRepository _repository;

    public DeleteStudyPlanCommandHandler(IStudyPlanRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
        DeleteStudyPlanCommand request,
        CancellationToken cancellationToken)
    {
        var plan = await _repository.GetStudyPlanByIdAsync(
            request.PlanId,
            request.UserId);

        if (plan == null)
            return Unit.Value;

        await _repository.DeleteStudyPlanAsync(plan);

        return Unit.Value;
    }
}