using MediatR;

namespace AIStudyAssistant.Application.Features.Notes.Commands;

public class UpdateNoteCommand : IRequest
{
    public int NoteId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public int SubjectId { get; set; }

    public int UserId { get; set; }
}