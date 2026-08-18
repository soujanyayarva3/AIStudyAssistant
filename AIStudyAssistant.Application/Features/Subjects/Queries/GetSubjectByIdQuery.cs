using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Subjects.Queries;

public class GetSubjectByIdQuery : IRequest<Subject?>
{
    public int SubjectId { get; set; }
    public int UserId { get; set; }
}