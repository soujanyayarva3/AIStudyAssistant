using AIStudyAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace AIStudyAssistant.Application.Features.Subjects.Commands;

public class DeleteSubjectCommandHandler
    : IRequestHandler<DeleteSubjectCommand>
{
    private readonly ISubjectRepository _repository;

    public DeleteSubjectCommandHandler(ISubjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
    DeleteSubjectCommand request,
    CancellationToken cancellationToken)
    {
        var subject = await _repository.GetSubjectByIdAsync(
            request.SubjectId,
            request.UserId);

        if (subject == null)
            return Unit.Value;

        await _repository.DeleteSubjectAsync(subject);

        return Unit.Value;
    }
}