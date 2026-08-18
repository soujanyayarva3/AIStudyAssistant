using AIStudyAssistant.Application.Interfaces;
using AIStudyAssistant.Infrastructure.Data;
using AIStudyAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIStudyAssistant.Infrastructure.Services;

public class ProgressCalculationService : IProgressCalculationService
{
    private readonly ApplicationDbContext _context;

    public ProgressCalculationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task UpdateProgressAsync(int userId)
    {
        int totalSubjects = await _context.Subjects
            .CountAsync(s => s.UserId == userId);

        int totalNotes = await _context.Notes
            .CountAsync(n => n.UserId == userId);

        int totalPlans = await _context.StudyPlans
            .CountAsync(p => p.UserId == userId);

        int completedPlans = await _context.StudyPlans
            .CountAsync(p =>
                p.UserId == userId &&
                p.Status == "Completed");

        int totalQuizzes = await _context.Quizzes
            .CountAsync(q => q.UserId == userId);

        double averageQuizScore = 0;

        if (totalQuizzes > 0)
        {
            averageQuizScore = await _context.Quizzes
                .Where(q => q.UserId == userId)
                .AverageAsync(q => q.Score);
        }

        double progressPercentage = 0;

        if (totalPlans > 0)
        {
            progressPercentage =
                (double)completedPlans / totalPlans * 100;
        }

        var progress = await _context.Progresses
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (progress == null)
        {
            progress = new Progress
            {
                UserId = userId
            };

            _context.Progresses.Add(progress);
        }

        progress.TotalSubjects = totalSubjects;
        progress.TotalNotes = totalNotes;
        progress.TotalStudyPlans = totalPlans;
        progress.CompletedStudyPlans = completedPlans;
        progress.TotalQuizzes = totalQuizzes;
        progress.AverageQuizScore = averageQuizScore;
        progress.ProgressPercentage = progressPercentage;
        progress.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}