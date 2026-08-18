using AIStudyAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace AIStudyAssistant.Application.Features.Subjects.Commands;

public class UpdateSubjectCommandHandler
    : IRequestHandler<UpdateSubjectCommand>
{
    private readonly ISubjectRepository _repository;

    public UpdateSubjectCommandHandler(ISubjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
    UpdateSubjectCommand request,
    CancellationToken cancellationToken)
    {
        var subject = await _repository.GetSubjectByIdAsync(
            request.SubjectId,
            request.UserId);

        if (subject == null)
            return Unit.Value;

        subject.SubjectName = request.Dto.SubjectName;
        subject.Description = request.Dto.Description;

        await _repository.UpdateSubjectAsync(subject);

        return Unit.Value;
    }
}