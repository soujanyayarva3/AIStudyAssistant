using AIStudyAssistant.Domain.Entities;

namespace AIStudyAssistant.Application.Interfaces.Repositories;

public interface IQuizRepository
{
    Task<List<Quiz>> GetQuizzesAsync(int userId);

    Task<Quiz?> GetQuizByIdAsync(int quizId, int userId);

    Task<List<Quiz>> GetQuizzesByTopicAsync(
        int userId,
        string topic
    );

    Task<Quiz> CreateQuizAsync(Quiz quiz);

    Task UpdateQuizAsync(Quiz quiz);

    Task DeleteQuizAsync(Quiz quiz);
}