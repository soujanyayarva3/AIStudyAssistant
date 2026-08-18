using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Notes.Queries;

public class GetNoteByIdQueryHandler
    : IRequestHandler<GetNoteByIdQuery, Note?>
{
    private readonly INoteRepository _repository;

    public GetNoteByIdQueryHandler(INoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Note?> Handle(
        GetNoteByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetNoteByIdAsync(
            request.NoteId,
            request.UserId);
    }
}