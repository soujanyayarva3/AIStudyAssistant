using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.StudyPlans.Queries;

public class GetStudyPlanByIdQuery : IRequest<StudyPlan?>
{
    public int PlanId { get; set; }

    public int UserId { get; set; }
}