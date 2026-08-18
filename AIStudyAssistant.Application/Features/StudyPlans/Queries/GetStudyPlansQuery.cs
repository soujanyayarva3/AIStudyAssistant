using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.StudyPlans.Queries;

public record GetStudyPlansQuery(int UserId) : IRequest<List<StudyPlan>>;