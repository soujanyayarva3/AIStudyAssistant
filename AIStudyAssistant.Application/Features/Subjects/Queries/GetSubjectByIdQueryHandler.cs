using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Subjects.Queries;

public class GetSubjectByIdQueryHandler
    : IRequestHandler<GetSubjectByIdQuery, Subject?>
{
    private readonly ISubjectRepository _repository;

    public GetSubjectByIdQueryHandler(ISubjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Subject?> Handle(
        GetSubjectByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetSubjectByIdAsync(
            request.SubjectId,
            request.UserId);
    }
}