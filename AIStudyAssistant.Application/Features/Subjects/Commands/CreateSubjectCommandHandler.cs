using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Subjects.Commands;

public class CreateSubjectCommandHandler
    : IRequestHandler<CreateSubjectCommand, Subject>
{
    private readonly ISubjectRepository _repository;

    public CreateSubjectCommandHandler(ISubjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Subject> Handle(
        CreateSubjectCommand request,
        CancellationToken cancellationToken)
    {
        var subject = new Subject
        {
            SubjectName = request.Dto.SubjectName,
            Description = request.Dto.Description,
            UserId = request.UserId
        };

        return await _repository.CreateSubjectAsync(subject);
    }
}