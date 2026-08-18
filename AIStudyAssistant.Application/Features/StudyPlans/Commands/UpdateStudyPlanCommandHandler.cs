using AIStudyAssistant.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.VisualBasic;

namespace AIStudyAssistant.Application.Features.StudyPlans.Commands;

public class UpdateStudyPlanCommandHandler
    : IRequestHandler<UpdateStudyPlanCommand>
{
    private readonly IStudyPlanRepository _repository;

    public UpdateStudyPlanCommandHandler(IStudyPlanRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
        UpdateStudyPlanCommand request,
        CancellationToken cancellationToken)
    {
        var plan = await _repository.GetStudyPlanByIdAsync(
            request.PlanId,
            request.UserId);

        if (plan == null)
            return Unit.Value;

        plan.TaskName = request.TaskName;
        plan.Description = request.Description;

        // Convert to UTC
        plan.StartDate = DateTime.SpecifyKind(
    request.StartDate.ToUniversalTime(),
    DateTimeKind.Utc);

        plan.DueDate = DateTime.SpecifyKind(
            request.DueDate.ToUniversalTime(),
            DateTimeKind.Utc);
        plan.Status = request.Status;

        await _repository.UpdateStudyPlanAsync(plan);

        return Unit.Value;
    }
}