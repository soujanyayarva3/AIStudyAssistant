
using System.Text.Json;

using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Application.Models;
using AIStudyAssistant.Application.Services;
using AIStudyAssistant.Domain.Entities;

using MediatR;

namespace AIStudyAssistant.Application.Features.Summaries.Commands;

public class CreateSummaryCommandHandler
    : IRequestHandler<CreateSummaryCommand, Summary>
{
    private readonly ISummaryRepository _repository;
    private readonly OllamaService _ollamaService;

    public CreateSummaryCommandHandler(
        ISummaryRepository repository,
        OllamaService ollamaService)
    {
        _repository = repository;
        _ollamaService = ollamaService;
    }

    public async Task<Summary> Handle(
        CreateSummaryCommand request,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            "=========================================="
        );

        Console.WriteLine(
            "CREATE SUMMARY HANDLER"
        );

        Console.WriteLine(
            $"Title: {request.Title}"
        );

        Console.WriteLine(
            $"UserId: {request.UserId}"
        );

        Console.WriteLine(
            $"SubjectId: {request.SubjectId}"
        );

        Console.WriteLine(
            $"SummaryStyle: {request.SummaryStyle}"
        );

        Console.WriteLine(
            $"OriginalText Length: {request.OriginalText?.Length}"
        );

        Console.WriteLine(
            "=========================================="
        );

        // =====================================================
        // AI PROMPT
        // =====================================================

        var prompt = $@"
You are an AI Study Assistant.

Generate a study summary based on the selected style.

Summary Style: {request.SummaryStyle}

Rules:

If the style is 'revision':

- Create concise exam revision notes.
- Highlight important concepts.
- Keep it short and easy to revise.

If the style is 'easy':

- Explain the topic in very simple language.
- Imagine teaching a beginner.
- Use short sentences.

If the style is 'interview':

- Focus on interview concepts.
- Mention important technical points.
- Include practical explanations useful for interviews.

IMPORTANT:

The value of ""summary"" MUST be a plain text string.

Do NOT make ""summary"" an object.

Do NOT create nested objects.

Return ONLY valid JSON.

Use exactly this structure:

{{
  ""summary"": ""Write the complete summary here as plain text."",
  ""keywords"": [
    ""keyword1"",
    ""keyword2"",
    ""keyword3"",
    ""keyword4"",
    ""keyword5""
  ],
  ""questions"": [
    ""Question 1?"",
    ""Question 2?"",
    ""Question 3?""
  ]
}}

Study Material:

{request.OriginalText}
";

        // =====================================================
        // CALL OLLAMA
        // =====================================================

        var aiResponse =
            await _ollamaService
                .GenerateResponseAsync(prompt);

        Console.WriteLine(
            "========== OLLAMA RESPONSE =========="
        );

        Console.WriteLine(
            aiResponse
        );

        Console.WriteLine(
            "======================================"
        );

        // =====================================================
        // CLEAN RESPONSE
        // =====================================================

        aiResponse =
            aiResponse
                .Replace("```json", "")
                .Replace("```JSON", "")
                .Replace("```", "")
                .Trim();

        // =====================================================
        // EXTRACT JSON
        // =====================================================

        var start =
            aiResponse.IndexOf("{");

        var end =
            aiResponse.LastIndexOf("}");

        if (
            start >= 0 &&
            end > start
        )
        {
            aiResponse =
                aiResponse.Substring(
                    start,
                    end - start + 1
                );
        }

        // =====================================================
        // PARSE AI RESPONSE
        // =====================================================

        SummaryAIResponse result;

        try
        {
            result =
                JsonSerializer.Deserialize<SummaryAIResponse>(
                    aiResponse,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                )
                ?? new SummaryAIResponse();

        }
        catch (Exception ex)
        {
            Console.WriteLine(
                "JSON PARSE ERROR:"
            );

            Console.WriteLine(
                ex.Message
            );

            result =
                new SummaryAIResponse
                {
                    Summary =
                        aiResponse,

                    Keywords =
                        new List<string>(),

                    Questions =
                        new List<string>()
                };
        }

        // =====================================================
        // SAFE VALUES
        // =====================================================

        var summaryText =
            result.Summary ?? string.Empty;

        var keywordList =
            result.Keywords ??
            new List<string>();

        var questionList =
            result.Questions ??
            new List<string>();

        // =====================================================
        // CREATE DATABASE ENTITY
        // =====================================================

        var summary =
            new Summary
            {
                Title =
                    request.Title,

                SubjectId =
                    request.SubjectId,

                SummaryStyle =
                    request.SummaryStyle,

                OriginalText =
                    request.OriginalText,

                SummaryText =
                    summaryText,

                Keywords =
                    JsonSerializer.Serialize(
                        keywordList
                    ),

                Questions =
                    JsonSerializer.Serialize(
                        questionList
                    ),

                UserId =
                    request.UserId,

                CreatedDate =
                    DateTime.UtcNow,

                IsGenerated =
                    true
            };

        // =====================================================
        // DATABASE DEBUG
        // =====================================================

        Console.WriteLine(
            "=========================================="
        );

        Console.WriteLine(
            "BEFORE DATABASE SAVE"
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
            $"SummaryText Length: {summary.SummaryText.Length}"
        );

        Console.WriteLine(
            $"Keywords: {summary.Keywords}"
        );

        Console.WriteLine(
            $"Questions: {summary.Questions}"
        );

        Console.WriteLine(
            "=========================================="
        );

        // =====================================================
        // SAVE TO DATABASE
        // =====================================================

        var savedSummary =
            await _repository
                .CreateSummaryAsync(
                    summary
                );

        // =====================================================
        // AFTER SAVE
        // =====================================================

        Console.WriteLine(
            "=========================================="
        );

        Console.WriteLine(
            "SUMMARY SAVED SUCCESSFULLY"
        );

        Console.WriteLine(
            $"SummaryId: {savedSummary.SummaryId}"
        );

        Console.WriteLine(
            $"Title: {savedSummary.Title}"
        );

        Console.WriteLine(
            $"UserId: {savedSummary.UserId}"
        );

        Console.WriteLine(
            $"SubjectId: {savedSummary.SubjectId}"
        );

        Console.WriteLine(
            "=========================================="
        );

        return savedSummary;
    }
}

