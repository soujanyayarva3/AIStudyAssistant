using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Notes.Queries;

public record GetNotesQuery(int UserId) : IRequest<List<Note>>;