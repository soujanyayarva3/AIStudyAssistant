using MediatR;

namespace AIStudyAssistant.Application.Features.Subjects.Commands;

public class DeleteSubjectCommand : IRequest
{
    public int SubjectId { get; set; }

    public int UserId { get; set; }
}