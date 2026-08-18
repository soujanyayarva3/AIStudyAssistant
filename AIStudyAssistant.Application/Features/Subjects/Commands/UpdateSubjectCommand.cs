using AIStudyAssistant.Application.DTOs;
using MediatR;

namespace AIStudyAssistant.Application.Features.Subjects.Commands;

public class UpdateSubjectCommand : IRequest
{
    public int SubjectId { get; set; }

    public int UserId { get; set; }

    public CreateSubjectDto Dto { get; set; } = null!;
}