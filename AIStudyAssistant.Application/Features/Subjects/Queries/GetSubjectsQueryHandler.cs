using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Subjects.Queries;

public class GetSubjectsQueryHandler : IRequestHandler<GetSubjectsQuery, List<Subject>>
{
    private readonly ISubjectRepository _repository;

    public GetSubjectsQueryHandler(ISubjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Subject>> Handle(
    GetSubjectsQuery request,
    CancellationToken cancellationToken)
    {
        Console.WriteLine("Handler reached");

        return await _repository.GetSubjectsAsync(request.UserId);
    }
}