using AIStudyAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace AIStudyAssistant.Application.Features.Notes.Commands;

public class DeleteNoteCommandHandler
    : IRequestHandler<DeleteNoteCommand>
{
    private readonly INoteRepository _repository;

    public DeleteNoteCommandHandler(INoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
        DeleteNoteCommand request,
        CancellationToken cancellationToken)
    {
        var note = await _repository.GetNoteByIdAsync(
            request.NoteId,
            request.UserId);

        if (note == null)
            return Unit.Value;

        await _repository.DeleteNoteAsync(note);

        return Unit.Value;
    }
}