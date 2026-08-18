using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Notes.Queries;

public class GetNotesQueryHandler
    : IRequestHandler<GetNotesQuery, List<Note>>
{
    private readonly INoteRepository _repository;

    public GetNotesQueryHandler(INoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Note>> Handle(
        GetNotesQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetNotesAsync(request.UserId);
    }
}