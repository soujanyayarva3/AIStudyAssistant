using AIStudyAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace AIStudyAssistant.Application.Features.Notes.Commands;

public class UpdateNoteCommandHandler
    : IRequestHandler<UpdateNoteCommand>
{
    private readonly INoteRepository _repository;

    public UpdateNoteCommandHandler(INoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
        UpdateNoteCommand request,
        CancellationToken cancellationToken)
    {
        var note = await _repository.GetNoteByIdAsync(
            request.NoteId,
            request.UserId);

        if (note == null)
            return Unit.Value;

        note.Title = request.Title;
        note.Content = request.Content;
        note.SubjectId = request.SubjectId;

        await _repository.UpdateNoteAsync(note);

        return Unit.Value;
    }
}