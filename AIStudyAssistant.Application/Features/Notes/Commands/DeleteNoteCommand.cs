using MediatR;

namespace AIStudyAssistant.Application.Features.Notes.Commands;

public class DeleteNoteCommand : IRequest
{
    public int NoteId { get; set; }

    public int UserId { get; set; }
}