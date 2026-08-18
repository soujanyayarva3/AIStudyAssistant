using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Notes.Queries;

public class GetNoteByIdQuery : IRequest<Note?>
{
    public int NoteId { get; set; }

    public int UserId { get; set; }
}