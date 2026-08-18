using AIStudyAssistant.Domain.Entities;

namespace AIStudyAssistant.Application.Interfaces.Repositories;

public interface INoteRepository
{
    Task<List<Note>> GetNotesAsync(int userId);

    Task<Note?> GetNoteByIdAsync(int noteId, int userId);

    Task<Note> CreateNoteAsync(Note note);

    Task UpdateNoteAsync(Note note);

    Task DeleteNoteAsync(Note note);
}