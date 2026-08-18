using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using AIStudyAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIStudyAssistant.Infrastructure.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly ApplicationDbContext _context;

    public NoteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Note>> GetNotesAsync(int userId)
    {
        return await _context.Notes
            .Where(n => n.UserId == userId)
            .Include(n => n.Subject)
            .ToListAsync();
    }

    public async Task<Note?> GetNoteByIdAsync(int noteId, int userId)
    {
        return await _context.Notes
            .Include(n => n.Subject)
            .FirstOrDefaultAsync(n => n.NoteId == noteId && n.UserId == userId);
    }

    public async Task<Note> CreateNoteAsync(Note note)
    {
        _context.Notes.Add(note);
        await _context.SaveChangesAsync();
        return note;
    }

    public async Task UpdateNoteAsync(Note note)
    {
        _context.Notes.Update(note);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteNoteAsync(Note note)
    {
        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();
    }
}