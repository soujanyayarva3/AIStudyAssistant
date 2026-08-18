
using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Notes.Commands;

public class CreateNoteCommandHandler
    : IRequestHandler<CreateNoteCommand, Unit>
{
    private readonly INoteRepository _repository;

    public CreateNoteCommandHandler(
        INoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
        CreateNoteCommand request,
        CancellationToken cancellationToken)
    {
        var note = new Note
        {
            Title = request.Title,
            Content = request.Content,
            SubjectId = request.SubjectId,
            UserId = request.UserId
        };

        await _repository.CreateNoteAsync(note);

        return Unit.Value;
    }
}
