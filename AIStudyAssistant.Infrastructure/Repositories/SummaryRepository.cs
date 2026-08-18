using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using AIStudyAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIStudyAssistant.Infrastructure.Repositories;

public class SummaryRepository : ISummaryRepository
{
    private readonly ApplicationDbContext _context;

    public SummaryRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Summary>> GetSummariesAsync(
        int userId)
    {
        return await _context.Summaries
            .AsNoTracking()
            .Include(s => s.Subject)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedDate)
            .ToListAsync();
    }

    public async Task<Summary?> GetSummaryByIdAsync(
        int id,
        int userId)
    {
        return await _context.Summaries
            .AsNoTracking()
            .Include(s => s.Subject)
            .FirstOrDefaultAsync(
                s =>
                    s.SummaryId == id &&
                    s.UserId == userId
            );
    }

    public async Task<Summary> CreateSummaryAsync(
        Summary summary)
    {
        _context.Summaries.Add(summary);

        await _context.SaveChangesAsync();

        return summary;
    }

    public async Task UpdateSummaryAsync(
        Summary summary)
    {
        _context.Summaries.Update(summary);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteSummaryAsync(
        Summary summary)
    {
        _context.Summaries.Remove(summary);

        await _context.SaveChangesAsync();
    }
}