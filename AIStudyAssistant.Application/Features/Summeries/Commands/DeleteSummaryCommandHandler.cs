using AIStudyAssistant.Application.Interfaces.Repositories;
using MediatR;

namespace AIStudyAssistant.Application.Features.Summaries.Commands;

public class DeleteSummaryCommandHandler
    : IRequestHandler<DeleteSummaryCommand, Unit>
{
    private readonly ISummaryRepository _repository;

    public DeleteSummaryCommandHandler(
        ISummaryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
        DeleteSummaryCommand request,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"DELETE SUMMARY REQUEST: ID={request.SummaryId}, USER={request.UserId}"
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

            return Unit.Value;
        }

        Console.WriteLine(
            $"SUMMARY FOUND: {summary.SummaryId} - {summary.Title}"
        );

        await _repository.DeleteSummaryAsync(
            summary
        );

        Console.WriteLine(
            $"SUMMARY DELETE COMPLETED: {summary.SummaryId}"
        );

        return Unit.Value;
    }
}