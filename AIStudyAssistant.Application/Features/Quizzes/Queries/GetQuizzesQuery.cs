using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Quizzes.Queries;

public record GetQuizzesQuery(int UserId) : IRequest<List<Quiz>>;