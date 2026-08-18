using MediatR;

namespace AIStudyAssistant.Application.Features.StudyPlans.Commands;

public class DeleteStudyPlanCommand : IRequest
{
    public int PlanId { get; set; }

    public int UserId { get; set; }
}