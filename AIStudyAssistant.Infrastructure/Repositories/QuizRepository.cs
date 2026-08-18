using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using AIStudyAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIStudyAssistant.Infrastructure.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly ApplicationDbContext _context;

    public QuizRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Quiz>> GetQuizzesAsync(int userId)
    {
        return await _context.Quizzes
            .Where(q => q.UserId == userId)
            .ToListAsync();
    }
    public async Task<List<Quiz>> GetQuizzesByTopicAsync(
    int userId,
    string topic)
{
    return await _context.Quizzes
        .Where(q =>
            q.UserId == userId &&
            q.Title.ToLower() == topic.ToLower())
        .ToListAsync();
}

    public async Task<Quiz?> GetQuizByIdAsync(int quizId, int userId)
    {
        return await _context.Quizzes
            .FirstOrDefaultAsync(q => q.QuizId == quizId && q.UserId == userId);
    }

    public async Task<Quiz> CreateQuizAsync(Quiz quiz)
    {
        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync();
        return quiz;
    }

    public async Task UpdateQuizAsync(Quiz quiz)
    {
        _context.Quizzes.Update(quiz);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteQuizAsync(Quiz quiz)
    {
        _context.Quizzes.Remove(quiz);
        await _context.SaveChangesAsync();
    }
}