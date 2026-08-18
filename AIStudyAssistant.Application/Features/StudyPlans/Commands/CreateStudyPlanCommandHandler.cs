using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.StudyPlans.Commands;

public class CreateStudyPlanCommandHandler
    : IRequestHandler<CreateStudyPlanCommand, StudyPlan>
{
    private readonly IStudyPlanRepository _repository;

    public CreateStudyPlanCommandHandler(IStudyPlanRepository repository)
    {
        _repository = repository;
    }

    public async Task<StudyPlan> Handle(
        CreateStudyPlanCommand request,
        CancellationToken cancellationToken)
    {
        var plan = new StudyPlan
        {
            TaskName = request.TaskName,
            Description = request.Description,

            StartDate = DateTime.SpecifyKind(
                request.StartDate.ToUniversalTime(),
                DateTimeKind.Utc),

            DueDate = DateTime.SpecifyKind(
                request.DueDate.ToUniversalTime(),
                DateTimeKind.Utc),

            Status = request.Status,
            UserId = request.UserId
        };
        Console.WriteLine($"StartDate: {plan.StartDate.Kind}");
        Console.WriteLine($"DueDate: {plan.DueDate.Kind}");
        return await _repository.CreateStudyPlanAsync(plan);
    }
}