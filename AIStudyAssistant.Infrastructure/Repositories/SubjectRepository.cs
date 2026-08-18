using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using AIStudyAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIStudyAssistant.Infrastructure.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly ApplicationDbContext _context;

    public SubjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Subject>> GetSubjectsAsync(int userId)
    {
        return await _context.Subjects
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }

    public async Task<Subject?> GetSubjectByIdAsync(int subjectId, int userId)
    {
        return await _context.Subjects
            .Include(s => s.Notes)
            .FirstOrDefaultAsync(s => s.SubjectId == subjectId && s.UserId == userId);
    }

    public async Task<Subject> CreateSubjectAsync(Subject subject)
    {
        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();

        return subject;
    }
    public async Task UpdateSubjectAsync(Subject subject)
    {
        _context.Subjects.Update(subject);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSubjectAsync(Subject subject)
    {
        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
    }
}