using AIStudyAssistant.Application.Interfaces.Repositories;
using ProgressEntity = AIStudyAssistant.Domain.Entities.Progress;
using AIStudyAssistant.Domain.Entities;
using AIStudyAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIStudyAssistant.Infrastructure.Repositories;

public class ProgressRepository : IProgressRepository
{
    private readonly ApplicationDbContext _context;

    public ProgressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Progress?> GetProgressAsync(int userId)
    {
        return await _context.Progresses
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<Progress> CreateProgressAsync(Progress progress)
    {
        _context.Progresses.Add(progress);
        await _context.SaveChangesAsync();
        return progress;
    }

    public async Task UpdateProgressAsync(Progress progress)
    {
        _context.Progresses.Update(progress);
        await _context.SaveChangesAsync();
    }
}