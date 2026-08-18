using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Subjects.Queries;

public record GetSubjectsQuery(int UserId) : IRequest<List<Subject>>;