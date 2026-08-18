using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using MediatR;

namespace AIStudyAssistant.Application.Features.Summaries.Queries;

public class GetSummaryByIdQueryHandler
    : IRequestHandler<
        GetSummaryByIdQuery,
        Summary?>
{
    private readonly ISummaryRepository _repository;

    public GetSummaryByIdQueryHandler(
        ISummaryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Summary?> Handle(
        GetSummaryByIdQuery request,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            "================================"
        );

        Console.WriteLine(
            "GET SUMMARY BY ID"
        );

        Console.WriteLine(
            $"SummaryId: {request.SummaryId}"
        );

        Console.WriteLine(
            $"UserId: {request.UserId}"
        );

        Console.WriteLine(
            "================================"
        );

        var summary =
            await _repository.GetSummaryByIdAsync(
                request.SummaryId,
                request.UserId
            );

        if (summary == null)
        {
            Console.WriteLine(
                "SUMMARY NOT FOUND"
            );
        }
        else
        {
            Console.WriteLine(
                "SUMMARY FOUND"
            );

            Console.WriteLine(
                $"Title: {summary.Title}"
            );

            Console.WriteLine(
                $"SummaryId: {summary.SummaryId}"
            );
        }

        return summary;
    }
}