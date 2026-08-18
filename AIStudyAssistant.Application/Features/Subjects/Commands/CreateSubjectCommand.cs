using AIStudyAssistant.Application.DTOs;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Subjects.Commands;

public class CreateSubjectCommand : IRequest<Subject>
{
    public CreateSubjectDto Dto { get; set; } = null!;
    public int UserId { get; set; }
}