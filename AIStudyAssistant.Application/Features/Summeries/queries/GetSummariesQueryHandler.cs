
using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Summaries.Queries;

public class GetSummariesQueryHandler
    : IRequestHandler<GetSummariesQuery, List<Summary>>
{
    private readonly ISummaryRepository _repository;

    public GetSummariesQueryHandler(
        ISummaryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Summary>> Handle(
        GetSummariesQuery request,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("==========================================");
        Console.WriteLine("GET ALL SUMMARIES");
        Console.WriteLine($"USER ID: {request.UserId}");
        Console.WriteLine("==========================================");

        var summaries =
            await _repository.GetSummariesAsync(
                request.UserId
            );

        Console.WriteLine(
            $"TOTAL SUMMARIES FOUND: {summaries.Count}"
        );

        foreach (var summary in summaries)
        {
            Console.WriteLine(
                $"SummaryId: {summary.SummaryId}"
            );

            Console.WriteLine(
                $"Title: {summary.Title}"
            );

            Console.WriteLine(
                $"UserId: {summary.UserId}"
            );

            Console.WriteLine(
                $"SubjectId: {summary.SubjectId}"
            );

            Console.WriteLine(
                $"SummaryStyle: {summary.SummaryStyle}"
            );

            Console.WriteLine(
                "------------------------------------------"
            );
        }

        return summaries;
    }
}

